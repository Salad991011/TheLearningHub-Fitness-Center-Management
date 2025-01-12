using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheLearningHub_Fitness_Center_Management.Models;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ModelContext _context;

        public AdminDashboardController(ModelContext context)
        {
            _context = context;
        }

        // Admin Dashboard Action
        public IActionResult AdminDashboard()
        {
            return View("~/Views/AdminDashboard/AdminDashboard.cshtml");
        }

        // Profile Action
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
    }
}
