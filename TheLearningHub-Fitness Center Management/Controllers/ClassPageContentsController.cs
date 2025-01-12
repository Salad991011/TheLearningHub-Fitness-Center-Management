using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheLearningHub_Fitness_Center_Management.Models;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    [Authorize]
    public class ClassPageContentsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ModelContext _context;

        public ClassPageContentsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        // GET: ClassPageContents
        public async Task<IActionResult> Index()
        {
            var classPageContents = await _context.ClassPageContents.ToListAsync();
            return View(classPageContents);
        }

        // GET: ClassPageContents/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
                return NotFound();

            var classPageContent = await _context.ClassPageContents
                .FirstOrDefaultAsync(m => m.ClassPageId == id);

            if (classPageContent == null)
                return NotFound();

            return View(classPageContent);
        }

        // GET: ClassPageContents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClassPageContents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassPageId,BackgroundTitle1,BackgroundDesc1,BackgroundImageFile1,ClassesTitle,ClassesDesc")] ClassPageContent classPageContent)
        {
            if (ModelState.IsValid)
            {
                if (classPageContent.BackgroundImageFile1 != null)
                {
                    classPageContent.BackgroundImagePath1 = SaveImage(classPageContent.BackgroundImageFile1);
                }

                _context.Add(classPageContent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(classPageContent);
        }

        // GET: ClassPageContents/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
                return NotFound();

            var classPageContent = await _context.ClassPageContents.FindAsync(id);
            if (classPageContent == null)
                return NotFound();

            return View(classPageContent);
        }

        // POST: ClassPageContents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("ClassPageId,BackgroundTitle1,BackgroundDesc1,BackgroundImageFile1,BackgroundImagePath1,ClassesTitle,ClassesDesc")] ClassPageContent classPageContent)
        {
            if (id != classPageContent.ClassPageId)
                return NotFound();

            if (ModelState.IsValid)
            {
                if (classPageContent.BackgroundImageFile1 != null)
                {
                    classPageContent.BackgroundImagePath1 = SaveImage(classPageContent.BackgroundImageFile1);
                }

                try
                {
                    _context.Update(classPageContent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassPageContentExists(classPageContent.ClassPageId))
                        return NotFound();

                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(classPageContent);
        }

        // GET: ClassPageContents/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
                return NotFound();

            var classPageContent = await _context.ClassPageContents
                .FirstOrDefaultAsync(m => m.ClassPageId == id);

            if (classPageContent == null)
                return NotFound();

            return View(classPageContent);
        }

        // POST: ClassPageContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var classPageContent = await _context.ClassPageContents.FindAsync(id);

            if (classPageContent != null)
            {
                _context.ClassPageContents.Remove(classPageContent);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ClassPageContentExists(decimal id)
        {
            return _context.ClassPageContents.Any(e => e.ClassPageId == id);
        }

        // Helper Method to Save Image
        private string SaveImage(Microsoft.AspNetCore.Http.IFormFile imageFile)
        {
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
