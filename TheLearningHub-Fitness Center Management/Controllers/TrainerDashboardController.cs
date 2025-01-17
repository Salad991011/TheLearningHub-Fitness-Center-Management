using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheLearningHub_Fitness_Center_Management.Models;
using System.Linq;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    public class TrainerDashboardController : Controller
    {
        private readonly ModelContext _context;

        public TrainerDashboardController(ModelContext context)
        {
            _context = context;
        }

        public IActionResult TrainerDashboard()
        {
            var trainerId = HttpContext.Session.GetInt32("UserId");
            if (trainerId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var assignedUsers = _context.Users
                .Include(u => u.Subscriptions)
                    .ThenInclude(s => s.Plan)
                .Include(u => u.Routines)
                    .ThenInclude(r => r.Trainer)
                .Where(u => u.Routines.Any(r => r.TrainerId == trainerId))
                .ToList();

            ViewBag.AssignedUsers = assignedUsers ?? new List<User>();

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
