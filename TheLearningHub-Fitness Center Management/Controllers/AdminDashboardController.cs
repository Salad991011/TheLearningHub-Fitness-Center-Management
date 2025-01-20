using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;
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
            // Retrieve cleared classes from the session
            var clearedClassesJson = HttpContext.Session.GetString("ClearedClasses");
            var clearedClassIds = new List<decimal>();

            if (!string.IsNullOrEmpty(clearedClassesJson))
            {
                var clearedClasses = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Class>>(clearedClassesJson);
                clearedClassIds = clearedClasses.Select(c => c.Classid).ToList();
            }

            // Fetch statistics
            var activeMemberships = _context.Subscriptions.Count(s =>
                s.DateFrom.HasValue &&
                s.DateTo.HasValue &&
                s.DateFrom <= DateTime.Now &&
                s.DateTo >= DateTime.Now);

            var pendingClasses = _context.Classes
                .Include(c => c.User)
                .Where(c => !c.ISAPPROVED)
                .ToList();

            var classRequests = _context.Classes
                .Include(c => c.User)
                .Where(c => (c.APPROVALSTATUS == "Approved" || c.APPROVALSTATUS == "Rejected") &&
                            !clearedClassIds.Contains(c.Classid)) // Exclude cleared classes
                .ToList();

            // Fetch registered trainers
            ViewBag.Trainers = _context.Users
                .Where(u => u.RoleId == 3) // Replace '3' with the correct RoleId for trainers
                .ToList();

            ViewBag.ActiveMemberships = activeMemberships;
            ViewBag.PendingClasses = pendingClasses;
            ViewBag.ClassRequests = classRequests;
            ViewBag.TotalSales = _context.Paidplans.Sum(p => p.PlanPrice);
            ViewBag.ActiveMembers = _context.Users.Count();
            ViewBag.Feedbacks = _context.Contactus.ToList();

            return View("~/Views/AdminDashboard/AdminDashboard.cshtml");
        }
        public IActionResult Profile()
        {
            // Retrieve admin ID from session
            var adminId = HttpContext.Session.GetInt32("LoginId"); // Assuming LoginId is stored in the session

            if (adminId == null)
            {
                return RedirectToAction("Login", "Auth"); // Redirect to login if session is null
            }

            // Fetch admin data
            var admin = _context.Users
                .Include(u => u.Login) // Include navigation property
                .FirstOrDefault(u => u.LoginId == adminId);

            if (admin == null)
            {
                return NotFound(); // Return 404 if admin not found
            }

            return View(admin); // Pass admin data to the view
        }
    


[HttpPost]
        public async Task<IActionResult> ApproveClass(decimal ClassId)
        {
            var pendingClass = await _context.Classes.Include(c => c.User).FirstOrDefaultAsync(c => c.Classid == ClassId);

            if (pendingClass == null)
            {
                TempData["ErrorMessage"] = "Class not found.";
                return RedirectToAction("AdminDashboard");
            }

            try
            {
                // Update approval status
                pendingClass.APPROVALSTATUS = "Approved";
                pendingClass.ISAPPROVED = true;

                // Send notification email
                await SendEmailAsync(
                    pendingClass.User.Email,
                    "Class Approval Notification",
                    $"Dear {pendingClass.User.Fname},\n\nYour class '{pendingClass.Classname}' has been approved."
                );

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Class '{pendingClass.Classname}' approved successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to approve class: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to approve class. Please try again.";
            }

            return RedirectToAction("AdminDashboard");
        }

        [HttpPost]
        public async Task<IActionResult> RejectClass(decimal ClassId)
        {
            var pendingClass = await _context.Classes.Include(c => c.User).FirstOrDefaultAsync(c => c.Classid == ClassId);

            if (pendingClass == null)
            {
                TempData["ErrorMessage"] = "Class not found.";
                return RedirectToAction("AdminDashboard");
            }

            try
            {
                // Update rejection status
                pendingClass.APPROVALSTATUS = "Rejected";
                pendingClass.ISAPPROVED = false;

                // Send notification email
                await SendEmailAsync(
                    pendingClass.User.Email,
                    "Class Rejection Notification",
                    $"Dear {pendingClass.User.Fname},\n\nUnfortunately, your class '{pendingClass.Classname}' has been rejected."
                );

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Class '{pendingClass.Classname}' rejected successfully.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to reject class: {ex.Message}");
                TempData["ErrorMessage"] = "Failed to reject class. Please try again.";
            }

            return RedirectToAction("AdminDashboard");
        }

        [HttpPost]
        public IActionResult ClearSelectedClasses([FromForm] List<decimal> selectedClassIds)
        {
            try
            {
                // Log the received IDs for debugging purposes
                Console.WriteLine($"Received Class IDs: {string.Join(", ", selectedClassIds ?? new List<decimal>())}");

                // Validate the input
                if (selectedClassIds == null || !selectedClassIds.Any())
                {
                    TempData["ErrorMessage"] = "Please select at least one class to clear.";
                    return RedirectToAction("AdminDashboard");
                }

                // Fetch the classes to be cleared
                var classesToClear = _context.Classes
                    .Include(c => c.User)
                    .Where(c => selectedClassIds.Contains(c.Classid))
                    .ToList();

                // Check if any matching classes were found
                if (!classesToClear.Any())
                {
                    TempData["ErrorMessage"] = "No matching classes found for the selected IDs.";
                    return RedirectToAction("AdminDashboard");
                }

                // Store cleared classes in the session
                var jsonSettings = new Newtonsoft.Json.JsonSerializerSettings
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                };
                HttpContext.Session.SetString("ClearedClasses", Newtonsoft.Json.JsonConvert.SerializeObject(classesToClear, jsonSettings));

                TempData["SuccessMessage"] = "Selected classes cleared from the view successfully.";
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error clearing classes: {ex.Message}");

                // Display an error message to the user
                TempData["ErrorMessage"] = "An error occurred while clearing the selected classes. Please try again.";
            }

            // Redirect back to the Admin Dashboard
            return RedirectToAction("AdminDashboard");
        }


        [HttpPost]
        public IActionResult RestoreClearedClasses()
        {
            try
            {
                var clearedClassesJson = HttpContext.Session.GetString("ClearedClasses");
                if (string.IsNullOrEmpty(clearedClassesJson))
                {
                    TempData["ErrorMessage"] = "No cleared classes to restore.";
                    return RedirectToAction("AdminDashboard");
                }

                HttpContext.Session.Remove("ClearedClasses");
                TempData["SuccessMessage"] = "Cleared classes restored to the view.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error restoring classes: {ex.Message}");
                TempData["ErrorMessage"] = "An error occurred while restoring the cleared classes. Please try again.";
            }

            return RedirectToAction("AdminDashboard");
        }


        private async Task SendEmailAsync(string recipientEmail, string subject, string body)
        {
            try
            {
                using var smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("Saladforyra@Saladsc.onmicrosoft.com", "Salad1234"),
                    EnableSsl = true
                };

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
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
