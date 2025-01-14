using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using MimeKit;
using MimeKit.Text;
using TheLearningHub_Fitness_Center_Management.Models;
using System.Net.Mail;
using System.Net;


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
            var activeMemberships = _context.Subscriptions.Count(s =>
                s.DateFrom.HasValue &&
                s.DateTo.HasValue &&
                s.DateFrom <= DateTime.Now &&
                s.DateTo >= DateTime.Now);

            var pendingClasses = _context.Classes
                .Include(c => c.User)
                .Where(c => c.ISAPPROVED == false)
                .ToList();

            var classRequests = _context.Classes
                .Include(c => c.User)
                .Where(c => c.APPROVALSTATUS == "Approved" || c.APPROVALSTATUS == "Rejected")
                .ToList();

            ViewBag.PendingClasses = pendingClasses;
            ViewBag.ClassRequests = classRequests;
            ViewBag.ActiveMemberships = activeMemberships;
            ViewBag.TotalSales = _context.Paidplans.Sum(p => p.PlanPrice);
            ViewBag.ActiveMembers = _context.Users.Count();
            return View("~/Views/AdminDashboard/AdminDashboard.cshtml");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            // Fetch admin user details based on session or other mechanism
            var adminId = HttpContext.Session.GetInt32("UserId");
            if (adminId == null)
            {
                TempData["ErrorMessage"] = "Admin not logged in.";
                return RedirectToAction("Login", "Auth");
            }

            var admin = _context.Users.FirstOrDefault(u => u.UserId == adminId && u.RoleId == 1);
            if (admin == null)
            {
                TempData["ErrorMessage"] = "Admin profile not found.";
                return RedirectToAction("AdminDashboard");
            }

            return View("~/Views/AdminDashboard/Profile.cshtml", admin);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClass(decimal ClassId)
        {
            // Fetch the pending class details
            var pendingClass = await _context.Classes
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Classid == ClassId);

            if (pendingClass != null)
            {
                try
                {
                    // Attempt to send an approval email
                    await SendEmailAsync(
                        pendingClass.User.Email,
                        "Class Approval Notification",
                        $"Dear {pendingClass.User.Fname},\n\nYour class '{pendingClass.Classname}' has been approved and added to the schedule."
                    );

                    // If email is sent successfully, update the class approval details
                    pendingClass.APPROVALSTATUS = "Approved";
                    pendingClass.ISAPPROVED = true;

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // If email fails, log the error and keep the request pending
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
            // Fetch the pending class details
            var pendingClass = await _context.Classes
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Classid == ClassId);

            if (pendingClass != null)
            {
                try
                {
                    // Attempt to send a rejection email
                    await SendEmailAsync(
                        pendingClass.User.Email,
                        "Class Rejection Notification",
                        $"Dear {pendingClass.User.Fname},\n\nUnfortunately, your class '{pendingClass.Classname}' has been rejected."
                    );

                    // If email is sent successfully, update the class rejection details
                    pendingClass.APPROVALSTATUS = "Rejected";
                    pendingClass.ISAPPROVED = false;

                    // Save changes to the database
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // If email fails, log the error and keep the request pending
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
                // Fetch all approved or rejected classes
                var classRequests = _context.Classes
                    .Where(c => c.APPROVALSTATUS == "Approved" || c.APPROVALSTATUS == "Rejected");

                // Remove the classes from the database
                _context.Classes.RemoveRange(classRequests);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "History cleared successfully.";
            }
            catch (Exception ex)
            {
                // Log error and show message if clearing fails
                Console.WriteLine($"Failed to clear history: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to clear history. Please try again.";
            }

            return RedirectToAction("AdminDashboard");
        }





    }
}



