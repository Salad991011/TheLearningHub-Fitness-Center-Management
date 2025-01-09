using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheLearningHub_Fitness_Center_Management.Models;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    public class ClassPageContentsController : Controller
    {
        private readonly ModelContext _context;

        public ClassPageContentsController(ModelContext context)
        {
            _context = context;
        }

        // GET: ClassPageContents
        public async Task<IActionResult> Index()
        {
              return _context.ClassPageContents != null ? 
                          View(await _context.ClassPageContents.ToListAsync()) :
                          Problem("Entity set 'ModelContext.ClassPageContents'  is null.");
        }

        // GET: ClassPageContents/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.ClassPageContents == null)
            {
                return NotFound();
            }

            var classPageContent = await _context.ClassPageContents
                .FirstOrDefaultAsync(m => m.ClassPageId == id);
            if (classPageContent == null)
            {
                return NotFound();
            }

            return View(classPageContent);
        }

        // GET: ClassPageContents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ClassPageContents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClassPageId,BackgroundTitle1,BackgroundDesc1,BackgroundImagePath1,ClassesTitle,ClassesDesc")] ClassPageContent classPageContent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(classPageContent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(classPageContent);
        }

        // GET: ClassPageContents/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.ClassPageContents == null)
            {
                return NotFound();
            }

            var classPageContent = await _context.ClassPageContents.FindAsync(id);
            if (classPageContent == null)
            {
                return NotFound();
            }
            return View(classPageContent);
        }

        // POST: ClassPageContents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("ClassPageId,BackgroundTitle1,BackgroundDesc1,BackgroundImagePath1,ClassesTitle,ClassesDesc")] ClassPageContent classPageContent)
        {
            if (id != classPageContent.ClassPageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(classPageContent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClassPageContentExists(classPageContent.ClassPageId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(classPageContent);
        }

        // GET: ClassPageContents/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.ClassPageContents == null)
            {
                return NotFound();
            }

            var classPageContent = await _context.ClassPageContents
                .FirstOrDefaultAsync(m => m.ClassPageId == id);
            if (classPageContent == null)
            {
                return NotFound();
            }

            return View(classPageContent);
        }

        // POST: ClassPageContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.ClassPageContents == null)
            {
                return Problem("Entity set 'ModelContext.ClassPageContents'  is null.");
            }
            var classPageContent = await _context.ClassPageContents.FindAsync(id);
            if (classPageContent != null)
            {
                _context.ClassPageContents.Remove(classPageContent);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClassPageContentExists(decimal id)
        {
          return (_context.ClassPageContents?.Any(e => e.ClassPageId == id)).GetValueOrDefault();
        }
    }
}
