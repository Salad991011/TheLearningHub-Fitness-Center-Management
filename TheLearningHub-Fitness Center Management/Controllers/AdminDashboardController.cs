using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            // Calculate active memberships based on Subscriptions where current date is between DateFrom and DateTo
            var activeMemberships = _context.Subscriptions.Count(s =>
                s.DateFrom.HasValue &&
                s.DateTo.HasValue &&
                s.DateFrom <= DateTime.Now &&
                s.DateTo >= DateTime.Now);
            /*
            var pendingClasses = _context.Classes
        .Where(c => c.ApprovalStatus == "Pending") // Update this condition based on your table structure
        .ToList();

            ViewBag.PendingClasses = pendingClasses;
            */

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

    }




}
