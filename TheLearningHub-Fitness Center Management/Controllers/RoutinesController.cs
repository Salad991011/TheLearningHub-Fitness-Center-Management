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
    public class RoutinesController : Controller
    {
        private readonly ModelContext _context;

        public RoutinesController(ModelContext context)
        {
            _context = context;
        }

        // GET: Routines
        public async Task<IActionResult> Index()
        {
              return _context.Routines != null ? 
                          View(await _context.Routines.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Routines'  is null.");
        }

        // GET: Routines/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Routines == null)
            {
                return NotFound();
            }

            var routine = await _context.Routines
                .FirstOrDefaultAsync(m => m.RoutineId == id);
            if (routine == null)
            {
                return NotFound();
            }

            return View(routine);
        }

        // GET: Routines/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Routines/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RoutineId,RoutineTime,Desc,ImagePath")] Routine routine)
        {
            if (ModelState.IsValid)
            {
                _context.Add(routine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(routine);
        }

        // GET: Routines/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Routines == null)
            {
                return NotFound();
            }

            var routine = await _context.Routines.FindAsync(id);
            if (routine == null)
            {
                return NotFound();
            }
            return View(routine);
        }

        // POST: Routines/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("RoutineId,RoutineTime,Desc,ImagePath")] Routine routine)
        {
            if (id != routine.RoutineId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(routine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoutineExists(routine.RoutineId))
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
            return View(routine);
        }

        // GET: Routines/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Routines == null)
            {
                return NotFound();
            }

            var routine = await _context.Routines
                .FirstOrDefaultAsync(m => m.RoutineId == id);
            if (routine == null)
            {
                return NotFound();
            }

            return View(routine);
        }

        // POST: Routines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Routines == null)
            {
                return Problem("Entity set 'ModelContext.Routines'  is null.");
            }
            var routine = await _context.Routines.FindAsync(id);
            if (routine != null)
            {
                _context.Routines.Remove(routine);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RoutineExists(decimal id)
        {
          return (_context.Routines?.Any(e => e.RoutineId == id)).GetValueOrDefault();
        }
    }
}
