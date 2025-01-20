using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using TheLearningHub_Fitness_Center_Management.Models;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    public class HomeController : Controller
    {
        private readonly ModelContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ModelContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewBag.BackgroundImagePath1 = _context.HomePageContents.Select(x => x.BackgroundImagePath1).FirstOrDefault();
            ViewBag.BackgroundImagePath2 = _context.HomePageContents.Select(x => x.BackgroundImagePath2).FirstOrDefault();
            ViewBag.ServiceImagePath2 = _context.HomePageContents.Select(x => x.ServicesImagePath2).FirstOrDefault();

            ViewBag.BACKGROUND_TITLE1 = _context.HomePageContents.Select(x => x.BackgroundTitle1).FirstOrDefault();
            ViewBag.BACKGROUND_DESC1 = _context.HomePageContents.Select(x => x.BackgroundDesc1).FirstOrDefault();
            ViewBag.COURSEL_TEXT_TITLE = _context.HomePageContents.Select(x => x.CourselTextTitle).FirstOrDefault();
            ViewBag.COURSEL_DESC = _context.HomePageContents.Select(x => x.CourselDesc).FirstOrDefault();
            ViewBag.BACKGROUND_TITLE2 = _context.HomePageContents.Select(x => x.BackgroundTitle2).FirstOrDefault();
            ViewBag.BACKGROUND_DESC2 = _context.HomePageContents.Select(x => x.BackgroundDesc2).FirstOrDefault();
            ViewBag.TRAINER_TITLE = _context.HomePageContents.Select(x => x.TrainerTitle).FirstOrDefault();
            ViewBag.TRAINER_DESC = _context.HomePageContents.Select(x => x.TrainerDesc).FirstOrDefault();

            ViewBag.Services = _context.Services.ToList();

            ViewBag.Trainers = _context.Users
                .Where(u => u.RoleId == 3) // Replace '3' with the correct RoleId for trainers
                .ToList();

            var plans = _context.Paidplans
                .Include(p => p.Trainer)
                .ToList();

            foreach (var plan in plans)
            {
                if (plan.PlanPrice.HasValue)
                {
                    plan.PlanPrice3Months = plan.PlanPrice * 1.5m;
                    plan.PlanPrice1Year = plan.PlanPrice * 1.9m;
                }
            }
            var homePageContent = _context.HomePageContents.FirstOrDefault()
         ?? new HomePageContent
         {
             FooterTitle = "Default Footer Title",
             FooterTitleDesc = "Default Footer Description",
             FooterEmail = "default@example.com"
         };
            ViewBag.FooterTitle = homePageContent.FooterTitle ?? "About Workout";
            ViewBag.FooterDescription = homePageContent.FooterTitleDesc
                ?? "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Veniam facere optio eligendi.";
            ViewBag.FooterEmail = homePageContent.FooterEmail ?? "default@example.com";

            ViewBag.FooterTitle = homePageContent.BackgroundTitle1 ?? "About Workout";
            ViewBag.FooterDescription = homePageContent.BackgroundDesc1
                ?? "Lorem ipsum dolor sit amet, consectetur adipisicing elit. Veniam facere optio eligendi.";
            ViewBag.SubscribeMessage = "Stay updated with our latest services and offers.";
            ViewBag.SubscribeEmailPlaceholder = "Email"; // Placeholder text
            return View(plans);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(Contactu feedback)
        {
            if (ModelState.IsValid)
            {
                _context.Contactus.Add(feedback);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Your feedback has been submitted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to submit feedback. Please try again.";
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Schedule(string search)
        {
            var pageContent = await _context.SchedulePageContents.FirstOrDefaultAsync()
                ?? new SchedulePageContent
                {
                    BackgroundTitle1 = "Default Title",
                    BackgroundDesc1 = "Default Description",
                    BackgroundImagePath1 = "default.jpg",
                    ScheduleTitle = "Default Schedule Title",
                    ScheduleDesc = "Default Schedule Description"
                };

            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return Unauthorized("User not logged in.");
            }

            var userSubscriptionsKey = $"User_{userId}_ClassSubscriptions";
            var subscribedClassIds = HttpContext.Session.Get<List<decimal>>(userSubscriptionsKey) ?? new List<decimal>();

            if (!subscribedClassIds.Any())
            {
                return View((pageContent, Enumerable.Empty<Schedule>()));
            }

            var schedulesQuery = _context.Schedules
                .Include(s => s.Routine)
                .ThenInclude(r => r.Trainer)
                .Include(s => s.Class)
                .Where(s => subscribedClassIds.Contains(s.ClassId ?? 0));

            IEnumerable<Schedule> schedules;

            if (!string.IsNullOrEmpty(search))
            {
                var schedulesList = await schedulesQuery.ToListAsync();
                schedules = schedulesList.Where(s =>
                    s.Routine?.Desc.ToLower().Contains(search.ToLower()) == true ||
                    s.Day?.ToString("MMMM dd, yyyy").ToLower().Contains(search.ToLower()) == true ||
                    (s.Routine?.Trainer != null &&
                     $"{s.Routine.Trainer.Fname} {s.Routine.Trainer.Lname}".ToLower().Contains(search.ToLower()))
                );
            }
            else
            {
                schedules = await schedulesQuery.ToListAsync();
            }

            return View((pageContent, schedules));
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Classes(string search)
        {
            var pageContent = await _context.ClassPageContents.FirstOrDefaultAsync()
                ?? new ClassPageContent
                {
                    BackgroundTitle1 = "Default Title",
                    BackgroundDesc1 = "Default Description",
                    BackgroundImagePath1 = "default.jpg",
                    ClassesTitle = "Default Classes Title",
                    ClassesDesc = "Default Classes Description"
                };

            // Fetch all approved classes
            var classesQuery = _context.Classes
                .Include(c => c.User) // Include trainer details
                .Where(c => c.ISAPPROVED == true);

            IEnumerable<Class> classes;

            // If a search term is provided, apply the filter after fetching data in memory
            if (!string.IsNullOrEmpty(search))
            {
                var classesList = await classesQuery.ToListAsync(); // Fetch all classes into memory
                classes = classesList
                    .Where(c =>
                        c.Classname?.ToLower().Contains(search.ToLower()) == true ||
                        c.Classdate?.ToString("MMMM dd, yyyy").ToLower().Contains(search.ToLower()) == true ||
                        (c.User != null && $"{c.User.Fname} {c.User.Lname}".ToLower().Contains(search.ToLower()))
                    );
            }
            else
            {
                // No search term, fetch directly from the database
                classes = await classesQuery.ToListAsync();
            }

            return View("Classes", (pageContent, classes));
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubscribeToClass(decimal classId)
        {
            var userId = int.Parse(User.Claims.First(c => c.Type == "UserId").Value);

            var hasActivePlan = await _context.Subscriptions
                .AnyAsync(s => s.UserId == userId && s.DateFrom <= DateTime.Now && s.DateTo >= DateTime.Now);

            if (!hasActivePlan)
            {
                TempData["ErrorMessage"] = "You must have an active plan to subscribe to a class.";
                return RedirectToAction("Classes");
            }

            var selectedClass = await _context.Classes
                .Include(c => c.Schedules)
                .FirstOrDefaultAsync(c => c.Classid == classId);

            if (selectedClass == null)
            {
                TempData["ErrorMessage"] = "The selected class does not exist.";
                return RedirectToAction("Classes");
            }

            if (!selectedClass.Schedules.Any())
            {
                TempData["ErrorMessage"] = "The selected class does not have any schedules.";
                return RedirectToAction("Classes");
            }

            var userSubscriptionsKey = $"User_{userId}_ClassSubscriptions";
            var subscribedClasses = HttpContext.Session.Get<List<decimal>>(userSubscriptionsKey) ?? new List<decimal>();

            if (subscribedClasses.Contains(classId))
            {
                TempData["ErrorMessage"] = "You are already subscribed to this class.";
                return RedirectToAction("Classes");
            }

            subscribedClasses.Add(classId);
            HttpContext.Session.Set(userSubscriptionsKey, subscribedClasses);

            TempData["SuccessMessage"] = "You have successfully subscribed to the class!";
            return RedirectToAction("Schedule");
        }

        [HttpGet]
        public JsonResult CheckLogin()
        {
            var isLoggedIn = HttpContext.Session.GetInt32("UserId") != null;
            return Json(new { isLoggedIn });
        }

        public IActionResult TrainerDashboard()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
