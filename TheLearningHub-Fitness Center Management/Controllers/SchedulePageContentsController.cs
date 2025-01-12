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
    public class SchedulePageContentsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ModelContext _context;

        public SchedulePageContentsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        // GET: SchedulePageContents
        public async Task<IActionResult> Index()
        {
            var schedulePageContents = await _context.SchedulePageContents.ToListAsync();
            return View(schedulePageContents);
        }

        // GET: SchedulePageContents/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
                return NotFound();

            var schedulePageContent = await _context.SchedulePageContents
                .FirstOrDefaultAsync(m => m.SchedulePageId == id);

            if (schedulePageContent == null)
                return NotFound();

            return View(schedulePageContent);
        }

        // GET: SchedulePageContents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SchedulePageContents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SchedulePageId,BackgroundTitle1,BackgroundDesc1,ScheduleImageFile,ScheduleTitle,ScheduleDesc")] SchedulePageContent schedulePageContent)
        {
            if (ModelState.IsValid)
            {
                if (schedulePageContent.ScheduleImageFile != null)
                {
                    string fileName = SaveImage(schedulePageContent.ScheduleImageFile);
                    schedulePageContent.BackgroundImagePath1 = fileName;
                }

                _context.Add(schedulePageContent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(schedulePageContent);
        }

        // GET: SchedulePageContents/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
                return NotFound();

            var schedulePageContent = await _context.SchedulePageContents.FindAsync(id);

            if (schedulePageContent == null)
                return NotFound();

            return View(schedulePageContent);
        }

        // POST: SchedulePageContents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("SchedulePageId,BackgroundTitle1,BackgroundDesc1,ScheduleImageFile,BackgroundImagePath1,ScheduleTitle,ScheduleDesc")] SchedulePageContent schedulePageContent)
        {
            if (id != schedulePageContent.SchedulePageId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (schedulePageContent.ScheduleImageFile != null)
                    {
                        string fileName = SaveImage(schedulePageContent.ScheduleImageFile);
                        schedulePageContent.BackgroundImagePath1 = fileName;
                    }

                    _context.Update(schedulePageContent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchedulePageContentExists(schedulePageContent.SchedulePageId))
                        return NotFound();

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(schedulePageContent);
        }

        // GET: SchedulePageContents/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
                return NotFound();

            var schedulePageContent = await _context.SchedulePageContents
                .FirstOrDefaultAsync(m => m.SchedulePageId == id);

            if (schedulePageContent == null)
                return NotFound();

            return View(schedulePageContent);
        }

        // POST: SchedulePageContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var schedulePageContent = await _context.SchedulePageContents.FindAsync(id);

            if (schedulePageContent != null)
            {
                _context.SchedulePageContents.Remove(schedulePageContent);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool SchedulePageContentExists(decimal id)
        {
            return _context.SchedulePageContents.Any(e => e.SchedulePageId == id);
        }

        // Helper Method to Save Image
        private string SaveImage(Microsoft.AspNetCore.Http.IFormFile imageFile)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Guid.NewGuid().ToString() + '_' + imageFile.FileName;
            string path = Path.Combine(wwwRootPath + "/Images/", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                imageFile.CopyTo(fileStream);
            }

            return fileName;
        }
    }
}
