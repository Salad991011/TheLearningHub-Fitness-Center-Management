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
            // Calculate active memberships based on Subscriptions where current date is between DateFrom and DateTo
            var activeMemberships = _context.Subscriptions.Count(s =>
                s.DateFrom.HasValue &&
                s.DateTo.HasValue &&
                s.DateFrom <= DateTime.Now &&
                s.DateTo >= DateTime.Now);

            var pendingClasses = _context.Classes
                .Include(c => c.User) // Include related User data
                .Where(c => !c.ISAPPROVED) // Fetch only pending classes
                .ToList();

            ViewBag.PendingClasses = pendingClasses;
            ViewBag.ActiveMemberships = activeMemberships;
            ViewBag.TotalSales = _context.Paidplans.Sum(p => p.PlanPrice); // Assuming PaidPlans have a Price field
            ViewBag.ActiveMembers = _context.Users.Count();
            return View("~/Views/AdminDashboard/AdminDashboard.cshtml");
        }

        public IActionResult Profile()
        {
            var adminId = HttpContext.Session.GetInt32("LoginId");

            if (adminId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var admin = _context.Users.Include(u => u.Login)
                                      .FirstOrDefault(u => u.LoginId == adminId);

            if (admin == null)
            {
                return NotFound();
            }

            return View("~/Views/AdminDashboard/Profile.cshtml", admin);
        }
        [HttpGet]
        public IActionResult Search(string query)
        {
            var results = _context.Users.Where(u =>
                u.Fname.Contains(query) ||
                u.Lname.Contains(query) ||
                u.Email.Contains(query)).ToList();

            ViewBag.SearchResults = results;
            return View("~/Views/AdminDashboard/SearchResults.cshtml");
        }
        [HttpGet]
        public IActionResult Trainers(string trainerQuery = "")
        {
            // Get all trainers (users with RoleId = 3 in the Login table)
            var trainers = _context.Users
                .Where(u => u.Login != null && u.Login.RoleId == 3) // Assuming RoleId 3 is for trainers
                .AsQueryable();

            // If there's a search query, filter trainers by name or email
            if (!string.IsNullOrEmpty(trainerQuery))
            {
                trainers = trainers.Where(t =>
                    t.Fname.Contains(trainerQuery) ||
                    t.Lname.Contains(trainerQuery) ||
                    t.Email.Contains(trainerQuery));
            }

            // Pass the list of trainers to the ViewBag
            ViewBag.Trainers = trainers.ToList();
            return View("~/Views/AdminDashboard/AdminDashboard.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClass(decimal ClassId)
        {
            // Include both User and Login data in the query
            var pendingClass = await _context.Classes
                .Include(c => c.User)
                .ThenInclude(u => u.Login)
                .FirstOrDefaultAsync(c => c.Classid == ClassId);

            if (pendingClass != null && pendingClass.User != null && pendingClass.User.Login != null && pendingClass.User.Login.RoleId == 3) // Verifying trainer's RoleId
            {
                pendingClass.APPROVALSTATUS = "Approved";
                await _context.SaveChangesAsync();

                // Send approval email to the trainer
                await SendEmailAsync(
                    pendingClass.User.Email,
                    "Class Approval Notification",
                    $"Dear {pendingClass.User.Fname},\n\nYour class '{pendingClass.Classname}' has been approved by the admin."
                );
            }

            return RedirectToAction("AdminDashboard");
        }

        [HttpPost]
        public async Task<IActionResult> RejectClass(decimal ClassId)
        {
            // Include both User and Login data in the query
            var pendingClass = await _context.Classes
                .Include(c => c.User)
                .ThenInclude(u => u.Login)
                .FirstOrDefaultAsync(c => c.Classid == ClassId);

            if (pendingClass != null && pendingClass.User != null && pendingClass.User.Login != null && pendingClass.User.Login.RoleId == 3) // Verifying trainer's RoleId
            {
                pendingClass.APPROVALSTATUS = "Rejected";
                await _context.SaveChangesAsync();

                // Send rejection email to the trainer
                await SendEmailAsync(
                    pendingClass.User.Email,
                    "Class Rejection Notification",
                    $"Dear {pendingClass.User.Fname},\n\nUnfortunately, your class '{pendingClass.Classname}' has been rejected by the admin."
                );
            }

            return RedirectToAction("AdminDashboard");
        }


        private async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                // Assuming you are using Microsoft.AspNetCore.Identity.UI.Services or similar email service
                using (var smtpClient = new SmtpClient("smtp.office365.com"))
                {
                    smtpClient.Port = 587; // Replace with your SMTP port
                    smtpClient.Credentials = new NetworkCredential("Saladforyra@Saladsc.onmicrosoft.com", "Salad1234"); // Replace with your email credentials
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
            catch (Exception ex)
            {
                // Log or handle the email sending failure
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }

    }







}



