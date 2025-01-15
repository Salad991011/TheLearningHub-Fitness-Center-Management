using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheLearningHub_Fitness_Center_Management.Models;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    public class HomePageContentsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ModelContext _context;

        public HomePageContentsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        // GET: HomePageContents
        public async Task<IActionResult> Index()
        {
            var homePageContents = await _context.HomePageContents.ToListAsync();
            return View(homePageContents);
        }

        // GET: HomePageContents/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
                return NotFound();

            var homePageContent = await _context.HomePageContents.FirstOrDefaultAsync(m => m.HomePageId == id);

            if (homePageContent == null)
                return NotFound();

            return View(homePageContent);
        }

        // GET: HomePageContents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HomePageContents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HomePageId,MainTitle,BackgroundTitle1,BackgroundDesc1,BackgroundImageFile1,CourselTextTitle,CourselDesc,CourselItemsTitle,CourselItemsDesc,BackgroundTitle2,BackgroundDesc2,BackgroundImageFile2,TrainerTitle,TrainerDesc,ServicesTitle,ServicesDesc1,ServicesImageFile2,ServicesDesc2,FooterTitle,FooterTitleDesc,FooterEmail")] HomePageContent homePageContent)
        {
            if (ModelState.IsValid)
            {
                homePageContent.BackgroundImagePath1 = SaveImage(homePageContent.BackgroundImageFile1);
                homePageContent.BackgroundImagePath2 = SaveImage(homePageContent.BackgroundImageFile2);
                homePageContent.ServicesImagePath2 = SaveImage(homePageContent.ServicesImageFile2);

                _context.Add(homePageContent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(homePageContent);
        }

        // GET: HomePageContents/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
                return NotFound();

            var homePageContent = await _context.HomePageContents.FindAsync(id);

            if (homePageContent == null)
                return NotFound();

            return View(homePageContent);
        }

        // POST: HomePageContents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("HomePageId,MainTitle,BackgroundTitle1,BackgroundDesc1,BackgroundImageFile1,CourselTextTitle,CourselDesc,CourselItemsTitle,CourselItemsDesc,BackgroundTitle2,BackgroundDesc2,BackgroundImageFile2,TrainerTitle,TrainerDesc,ServicesTitle,ServicesDesc1,ServicesImageFile2,ServicesDesc2,FooterTitle,FooterTitleDesc,FooterEmail")] HomePageContent homePageContent)
        {
            if (id != homePageContent.HomePageId)
                return NotFound();

            if (ModelState.IsValid)
            {
                homePageContent.BackgroundImagePath1 = SaveImage(homePageContent.BackgroundImageFile1);
                homePageContent.BackgroundImagePath2 = SaveImage(homePageContent.BackgroundImageFile2);
                homePageContent.ServicesImagePath2 = SaveImage(homePageContent.ServicesImageFile2);

                try
                {
                    _context.Update(homePageContent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomePageContentExists(homePageContent.HomePageId))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(homePageContent);
        }

        // GET: HomePageContents/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
                return NotFound();

            var homePageContent = await _context.HomePageContents.FirstOrDefaultAsync(m => m.HomePageId == id);

            if (homePageContent == null)
                return NotFound();

            return View(homePageContent);
        }

        // POST: HomePageContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var homePageContent = await _context.HomePageContents.FindAsync(id);

            if (homePageContent != null)
            {
                _context.HomePageContents.Remove(homePageContent);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Dynamic content rendering for the Home page
        public async Task<IActionResult> RenderHomePage()
        {
            var homePageContent = await _context.HomePageContents.FirstOrDefaultAsync();
            if (homePageContent == null)
            {
                return View("Error", new { Message = "Home page content not found." });
            }

            // Pass data to the view using ViewBag or ViewData
            ViewBag.MainTitle = homePageContent.MainTitle;
            ViewBag.BackgroundTitle1 = homePageContent.BackgroundTitle1;
            ViewBag.BackgroundDesc1 = homePageContent.BackgroundDesc1;
            ViewBag.BackgroundImagePath1 = homePageContent.BackgroundImagePath1;
            ViewBag.TrainerTitle = homePageContent.TrainerTitle;
            ViewBag.ServicesDesc1 = homePageContent.ServicesDesc1;

            return View("Home", homePageContent); // Ensure the view exists as `Home.cshtml`
        }

        private bool HomePageContentExists(decimal id)
        {
            return _context.HomePageContents.Any(e => e.HomePageId == id);
        }

        // Helper Method to Save Image
        private string SaveImage(Microsoft.AspNetCore.Http.IFormFile imageFile)
        {
            if (imageFile == null) return null;

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string path = Path.Combine(wwwRootPath + "/Images/", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }

            return fileName;
        }
    }
}
