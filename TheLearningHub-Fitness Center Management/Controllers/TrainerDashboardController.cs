using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheLearningHub_Fitness_Center_Management.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    public class TrainerDashboardController : Controller
    {
        private readonly ModelContext _context;

        public TrainerDashboardController(ModelContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Trainer")]
        public IActionResult TrainerDashboard()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                TempData["ErrorMessage"] = "Unauthorized access.";
                return RedirectToAction("AccessDenied", "Auth");
            }

            var userId = int.Parse(userIdClaim.Value); // Trainer's ID

            // Total Classes Created by the Trainer
            var totalClassesCreated = _context.Classes
                .Where(c => c.Userid == userId) // Trainer's UserId matches the Class's Userid
                .Count();

            // Total Users Attending the Trainer's Classes
            var totalUsersAttendingClasses = _context.Routines
                .Where(r => r.TrainerId == userId) // Filter by TrainerId in Routines
                .Select(r => r.UserId)
                .Distinct()
                .Count();

            // Assigned users for this trainer
            var assignedUsers = _context.Users
                .Include(u => u.Subscriptions)
                .ThenInclude(s => s.Plan)
                .Include(u => u.Routines)
                .ThenInclude(r => r.Trainer)
                .Where(u => u.Routines.Any(r => r.TrainerId == userId)) // Users with Routines mapped to the Trainer
                .ToList();
            var usersAttendingClasses = _context.Users
    .Include(u => u.Subscriptions)
    .ThenInclude(s => s.Plan)
    .Where(u => _context.Routines.Any(r => r.TrainerId == userId && r.UserId == u.UserId))
    .ToList();

            // Pass the list to the view
            ViewBag.UsersAttendingClasses = usersAttendingClasses;
            ViewBag.TotalClassesCreated = totalClassesCreated;
            ViewBag.TotalUsersAttendingClasses = totalUsersAttendingClasses;
            ViewBag.AssignedUsers = assignedUsers;

            return View();
        }




        public IActionResult Profile()
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                TempData["ErrorMessage"] = "Unauthorized access.";
                return RedirectToAction("AccessDenied", "Auth");
            }

            var userId = int.Parse(userIdClaim.Value);

            var trainer = _context.Users.FirstOrDefault(u => u.UserId == userId);

            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Profile not found.";
                return RedirectToAction("TrainerDashboard");
            }

            return View(trainer);
        }

        public IActionResult Statistics()
        {
            // Retrieve UserId from claims
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (userIdClaim == null)
            {
                TempData["ErrorMessage"] = "Unauthorized access.";
                return RedirectToAction("AccessDenied", "Auth");
            }

            var trainerId = int.Parse(userIdClaim.Value); // Trainer's UserId

            // Calculate total users trained by the trainer
            var totalUsersTrained = _context.Routines
                .Where(r => r.TrainerId == trainerId) // Match trainer's ID in Routines
                .Select(r => r.UserId) // Get user IDs associated with this trainer
                .Distinct()
                .Count();

            // Calculate total classes conducted by the trainer
            var totalClassesConducted = _context.Classes
                .Where(c => _context.Routines.Any(r => r.TrainerId == trainerId && r.UserId == c.Userid)) // Match classes indirectly via Routines
                .Count();

            // Pass results to the view
            ViewBag.TotalUsersTrained = totalUsersTrained;
            ViewBag.TotalClassesConducted = totalClassesConducted;

            return View();
        }



        [HttpGet]
        public IActionResult EditWeeklySchedule(decimal userId)
        {
            // Fetch user and their routines
            var user = _context.Users
                .Include(u => u.Routines)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            // Pass user and their routines to the view
            ViewBag.User = user;
            ViewBag.Routines = user.Routines
                .GroupBy(r => r.RoutineTime.Value.DayOfWeek)
                .ToList();

            return View();
        }

        [HttpPost]
        public IActionResult UpdateWeeklySchedule(decimal userId, List<Routine> updatedRoutines)
        {
            var existingRoutines = _context.Routines
                .Where(r => r.UserId == userId)
                .ToList();

            foreach (var updatedRoutine in updatedRoutines)
            {
                var routine = existingRoutines.FirstOrDefault(r => r.RoutineId == updatedRoutine.RoutineId);
                if (routine != null)
                {
                    routine.Desc = updatedRoutine.Desc;
                    routine.RoutineTime = updatedRoutine.RoutineTime;
                }
                else
                {
                    _context.Routines.Add(updatedRoutine);
                }
            }

            _context.SaveChanges();

            return RedirectToAction("TrainerDashboard");
        }
    }
}
