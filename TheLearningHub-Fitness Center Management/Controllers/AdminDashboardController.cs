using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using TheLearningHub_Fitness_Center_Management.Models;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    [Route("Admin/[action]")]
    public class AdminDashboardController : Controller
    {
        private readonly ModelContext _context;

        public AdminDashboardController(ModelContext context)
        {
            _context = context;
        }

        public IActionResult AdminDashboard()
        {
            // Fetch statistics for dashboard
            var activeMemberships = _context.Subscriptions.Count(s =>
                s.DateFrom.HasValue &&
                s.DateTo.HasValue &&
                s.DateFrom <= DateTime.Now &&
                s.DateTo >= DateTime.Now);

            var pendingClasses = _context.Classes
                .Include(c => c.User)
                .ThenInclude(u => u.Role) // Include Role for debugging/insight
                .Where(c => c.ISAPPROVED == false)
                .ToList();

            var classRequests = _context.Classes
                .Include(c => c.User)
                .ThenInclude(u => u.Role) // Include Role for debugging/insight
                .Where(c => c.APPROVALSTATUS == "Approved" || c.APPROVALSTATUS == "Rejected")
                .ToList();
            var loginId = HttpContext.Session.GetInt32("LoginId");
            if (loginId != null)
            {
                var user = _context.Users.FirstOrDefault(u => u.LoginId == loginId);
                ViewData["ProfileImage"] = user?.ImagePath != null
                    ? Url.Content($"~/Images/{user.ImagePath}")
                    : Url.Content("~/AdminDesign/assets/images/faces/placeholder.png");
            }
            // Pass data to the view
            ViewBag.PendingClasses = pendingClasses;
            ViewBag.ClassRequests = classRequests;
            ViewBag.ActiveMemberships = activeMemberships;
            ViewBag.TotalSales = _context.Paidplans.Sum(p => p.PlanPrice);
            ViewBag.ActiveMembers = _context.Users.Count();
            
          
            var feedbacks = _context.Contactus.ToList();

            ViewBag.Feedbacks = feedbacks;

           


            return View("~/Views/AdminDashboard/AdminDashboard.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSelectedFeedback([FromForm] List<decimal> selectedFeedbackIds)
        {
            if (selectedFeedbackIds != null && selectedFeedbackIds.Any())
            {
                try
                {
                    var feedbacksToDelete = _context.Contactus
                        .Where(f => selectedFeedbackIds.Contains(f.ContactId))
                        .ToList();

                    if (feedbacksToDelete.Any())
                    {
                        _context.Contactus.RemoveRange(feedbacksToDelete);
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = "Selected feedback entries deleted successfully.";
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete selected feedback: {ex.Message}");
                    TempData["ErrorMessage"] = "Failed to delete selected feedback. Please try again.";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "No feedback entries selected for deletion.";
            }

            return RedirectToAction("AdminDashboard");
        }


        [HttpGet]
        public IActionResult Profile()
        {
            var adminId = HttpContext.Session.GetInt32("UserId");
            if (adminId == null)
            {
                Console.WriteLine("UserId in session is null.");
                TempData["ErrorMessage"] = "Admin not logged in.";
                return RedirectToAction("Login", "Auth");
            }

            var admin = _context.Users
                .Include(u => u.Role) // Include the Role navigation property
                .FirstOrDefault(u => u.UserId == adminId && u.RoleId == 1); // Ensure RoleId == 1 (Admin role)

            if (admin == null)
            {
                Console.WriteLine($"No admin found with UserId: {adminId}");
                TempData["ErrorMessage"] = "Admin profile not found.";
                return RedirectToAction("AdminDashboard");
            }

            Console.WriteLine($"Admin profile loaded successfully for UserId: {adminId}");
            return View("~/Views/AdminDashboard/Profile.cshtml", admin);
        }


        [HttpPost]
        public async Task<IActionResult> ApproveClass(decimal ClassId)
        {
            var pendingClass = await _context.Classes
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Classid == ClassId);

            if (pendingClass != null)
            {
                try
                {
                    await SendEmailAsync(
                        pendingClass.User.Email,
                        "Class Approval Notification",
                        $"Dear {pendingClass.User.Fname},\n\nYour class '{pendingClass.Classname}' has been approved and added to the schedule."
                    );

                    pendingClass.APPROVALSTATUS = "Approved";
                    pendingClass.ISAPPROVED = true;

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email for class approval: {ex.Message}");
                    TempData["ErrorMessage"] = "Failed to send approval email. Please refresh and try again.";
                    return RedirectToAction("AdminDashboard");
                }
            }

            return RedirectToAction("AdminDashboard");
        }

        [HttpPost]
        public async Task<IActionResult> RejectClass(decimal ClassId)
        {
            var pendingClass = await _context.Classes
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Classid == ClassId);

            if (pendingClass != null)
            {
                try
                {
                    await SendEmailAsync(
                        pendingClass.User.Email,
                        "Class Rejection Notification",
                        $"Dear {pendingClass.User.Fname},\n\nUnfortunately, your class '{pendingClass.Classname}' has been rejected."
                    );

                    pendingClass.APPROVALSTATUS = "Rejected";
                    pendingClass.ISAPPROVED = false;

                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send email for class rejection: {ex.Message}");
                    TempData["ErrorMessage"] = "Failed to send rejection email. Please refresh and try again.";
                    return RedirectToAction("AdminDashboard");
                }
            }

            return RedirectToAction("AdminDashboard");
        }

        private async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            using (var smtpClient = new SmtpClient("smtp.office365.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("Saladforyra@Saladsc.onmicrosoft.com", "Salad1234"); // Replace with valid credentials
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("Saladforyra@Saladsc.onmicrosoft.com", "Admin"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(recipientEmail);

                await smtpClient.SendMailAsync(mailMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> ClearHistory()
        {
            try
            {
                var classRequests = _context.Classes
                    .Where(c => c.APPROVALSTATUS == "Approved" || c.APPROVALSTATUS == "Rejected");

                _context.Classes.RemoveRange(classRequests);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "History cleared successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clear history: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to clear history. Please try again.";
            }

            return RedirectToAction("AdminDashboard");
        }

        public async Task<IActionResult> ManageHomePageContent()
        {
            var contents = await _context.HomePageContents.ToListAsync();
            return View("~/Views/AdminDashboard/ManageHomePageContent.cshtml", contents);
        }
    }
}
