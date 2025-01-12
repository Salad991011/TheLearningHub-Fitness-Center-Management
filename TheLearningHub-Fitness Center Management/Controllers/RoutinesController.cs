using System;
using System.Collections.Generic;
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
    public class RoutinesController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ModelContext _context;

        public RoutinesController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        // GET: Routines
        public async Task<IActionResult> Index()
        {
            return View(await _context.Routines.ToListAsync());
        }

        // GET: Routines/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
                return NotFound();

            var routine = await _context.Routines.FirstOrDefaultAsync(m => m.RoutineId == id);
            if (routine == null)
                return NotFound();

            return View(routine);
        }

        // GET: Routines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Routines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoutineId,RoutineTime,Desc,RoutineImageFile")] Routine routine)
        {
            if (ModelState.IsValid)
            {
                if (routine.RoutineImageFile != null)
                {
                    string fileName = SaveImage(routine.RoutineImageFile);
                    routine.ImagePath = fileName;
                }

                _context.Add(routine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(routine);
        }

        // GET: Routines/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
                return NotFound();

            var routine = await _context.Routines.FindAsync(id);
            if (routine == null)
                return NotFound();

            return View(routine);
        }

        // POST: Routines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("RoutineId,RoutineTime,Desc,RoutineImageFile,ImagePath")] Routine routine)
        {
            if (id != routine.RoutineId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (routine.RoutineImageFile != null)
                    {
                        string fileName = SaveImage(routine.RoutineImageFile);
                        routine.ImagePath = fileName;
                    }

                    _context.Update(routine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoutineExists(routine.RoutineId))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(routine);
        }

        // GET: Routines/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
                return NotFound();

            var routine = await _context.Routines.FirstOrDefaultAsync(m => m.RoutineId == id);
            if (routine == null)
                return NotFound();

            return View(routine);
        }

        // POST: Routines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var routine = await _context.Routines.FindAsync(id);
            if (routine != null)
            {
                _context.Routines.Remove(routine);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool RoutineExists(decimal id)
        {
            return _context.Routines.Any(e => e.RoutineId == id);
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
