using Microsoft.AspNetCore.Authorization;
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
            var backgroundImage = _context.HomePageContents
          .Select(x => x.BackgroundImagePath1)
          .FirstOrDefault();

            ViewBag.BackgroundImagePath2 = _context.HomePageContents.Select(x => x.BackgroundImagePath2).FirstOrDefault();
            var backgroundImage2 = _context.HomePageContents
          .Select(x => x.BackgroundImagePath2)
          .FirstOrDefault();

            ViewBag.ServiceImagePath2 = _context.HomePageContents.Select(x => x.ServicesImagePath2).FirstOrDefault();
            var ServiceImagePath2 = _context.HomePageContents
          .Select(x => x.ServicesImagePath2)
          .FirstOrDefault();

            ViewBag.BACKGROUND_TITLE1 = _context.HomePageContents.Select(x => x.BackgroundTitle1).FirstOrDefault();
            ViewBag.BACKGROUND_DESC1 = _context.HomePageContents.Select(x => x.BackgroundDesc1).FirstOrDefault();
            ViewBag.COURSEL_TEXT_TITLE = _context.HomePageContents.Select(x => x.CourselTextTitle).FirstOrDefault();
            ViewBag.COURSEL_DESC = _context.HomePageContents.Select(x => x.CourselDesc).FirstOrDefault();
            ViewBag.COURSEL_ITEMS_TITLE = _context.HomePageContents.Select(x => x.CourselItemsTitle).FirstOrDefault();
            ViewBag.COURSEL_ITEMS_DESC = _context.HomePageContents.Select(x => x.CourselItemsDesc).FirstOrDefault();
        
            ViewBag.BACKGROUND_TITLE2 = _context.HomePageContents.Select(x => x.BackgroundTitle2).FirstOrDefault();
            ViewBag.BACKGROUND_DESC2 = _context.HomePageContents.Select(x => x.BackgroundDesc2).FirstOrDefault();
            ViewBag.TRAINER_TITLE = _context.HomePageContents.Select(x => x.TrainerTitle).FirstOrDefault();
            ViewBag.TRAINER_DESC = _context.HomePageContents.Select(x => x.TrainerDesc).FirstOrDefault();
            ViewBag.SERVICES_TITLE = _context.HomePageContents.Select(x => x.ServicesTitle).FirstOrDefault();
            ViewBag.SERVICES_DESC1 = _context.HomePageContents.Select(x => x.ServicesDesc1).FirstOrDefault();
            ViewBag.SERVICES_DESC2 = _context.HomePageContents.Select(x => x.ServicesDesc2).FirstOrDefault();
            // Handle if the database value is null or empty

            ViewBag.BackgroundImagePath1 = string.IsNullOrEmpty(backgroundImage) ? "default.jpg" : backgroundImage;

            ViewBag.BackgroundImagePath2 = string.IsNullOrEmpty(backgroundImage2) ? "default.jpg" : backgroundImage2;
            
            ViewBag.ServiceImagePath2 = string.IsNullOrEmpty(ServiceImagePath2) ? "default.jpg" : ServiceImagePath2;

            var services = _context.Services.ToList();
            ViewBag.Services = services;

            var trainers = _context.Users
        .Where(u => u.RoleId == 3) // Replace '2' with the actual RoleId for trainers
        .ToList();

            ViewBag.Trainers = trainers;

            var plans = _context.Paidplans
       .Include(p => p.Trainer) // Include Trainer navigation property
       .ToList();



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
		public IActionResult Schedule()
		{
			return View("~/Views/Home/Schedule.cshtml");
		}

		[Authorize]
		public IActionResult Classes()
		{
			return View("~/Views/Home/Classes.cshtml");
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
