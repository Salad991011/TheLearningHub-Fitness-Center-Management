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
    public class SchedulePageContentsController : Controller
    {
        private readonly ModelContext _context;

        public SchedulePageContentsController(ModelContext context)
        {
            _context = context;
        }

        // GET: SchedulePageContents
        public async Task<IActionResult> Index()
        {
              return _context.SchedulePageContents != null ? 
                          View(await _context.SchedulePageContents.ToListAsync()) :
                          Problem("Entity set 'ModelContext.SchedulePageContents'  is null.");
        }

        // GET: SchedulePageContents/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.SchedulePageContents == null)
            {
                return NotFound();
            }

            var schedulePageContent = await _context.SchedulePageContents
                .FirstOrDefaultAsync(m => m.SchedulePageId == id);
            if (schedulePageContent == null)
            {
                return NotFound();
            }

            return View(schedulePageContent);
        }

        // GET: SchedulePageContents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SchedulePageContents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SchedulePageId,BackgroundTitle1,BackgroundDesc1,BackgroundImagePath1,ScheduleTitle,ScheduleDesc")] SchedulePageContent schedulePageContent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(schedulePageContent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(schedulePageContent);
        }

        // GET: SchedulePageContents/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.SchedulePageContents == null)
            {
                return NotFound();
            }

            var schedulePageContent = await _context.SchedulePageContents.FindAsync(id);
            if (schedulePageContent == null)
            {
                return NotFound();
            }
            return View(schedulePageContent);
        }

        // POST: SchedulePageContents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("SchedulePageId,BackgroundTitle1,BackgroundDesc1,BackgroundImagePath1,ScheduleTitle,ScheduleDesc")] SchedulePageContent schedulePageContent)
        {
            if (id != schedulePageContent.SchedulePageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(schedulePageContent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SchedulePageContentExists(schedulePageContent.SchedulePageId))
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
            return View(schedulePageContent);
        }

        // GET: SchedulePageContents/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.SchedulePageContents == null)
            {
                return NotFound();
            }

            var schedulePageContent = await _context.SchedulePageContents
                .FirstOrDefaultAsync(m => m.SchedulePageId == id);
            if (schedulePageContent == null)
            {
                return NotFound();
            }

            return View(schedulePageContent);
        }

        // POST: SchedulePageContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.SchedulePageContents == null)
            {
                return Problem("Entity set 'ModelContext.SchedulePageContents'  is null.");
            }
            var schedulePageContent = await _context.SchedulePageContents.FindAsync(id);
            if (schedulePageContent != null)
            {
                _context.SchedulePageContents.Remove(schedulePageContent);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SchedulePageContentExists(decimal id)
        {
          return (_context.SchedulePageContents?.Any(e => e.SchedulePageId == id)).GetValueOrDefault();
        }
    }
}
