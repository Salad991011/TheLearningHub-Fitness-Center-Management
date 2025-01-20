using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheLearningHub_Fitness_Center_Management.Models;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    public class SchedulesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public SchedulesController(ModelContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Schedules
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Schedules
                .Include(s => s.Plan)
                .Include(s => s.Routine)
                .Include(s => s.Class);
            return View(await modelContext.ToListAsync());
        }

        // GET: Schedules/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Schedules == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Plan)
                .Include(s => s.Routine)
                .Include(s => s.Class)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // GET: Schedules/Create
        public IActionResult Create()
        {
            ViewData["PlanId"] = new SelectList(_context.Paidplans, "PlanId", "PlanTitle");
            ViewData["RoutineId"] = new SelectList(_context.Routines, "RoutineId", "Desc");
            ViewData["ClassId"] = new SelectList(_context.Classes, "Classid", "Classname");
            return View();
        }

        // POST: Schedules/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ScheduleId,Day,Time,ScheduleImageFile,PlanId,RoutineId,ClassId")] Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                if (schedule.ScheduleImageFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(schedule.ScheduleImageFile.FileName);
                    string filePath = Path.Combine(_hostEnvironment.WebRootPath, "Images", fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await schedule.ScheduleImageFile.CopyToAsync(fileStream);
                    }

                    schedule.ImagePath = fileName; // Save the file name to the database
                }

                _context.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlanId"] = new SelectList(_context.Paidplans, "PlanId", "PlanTitle", schedule.PlanId);
            ViewData["RoutineId"] = new SelectList(_context.Routines, "RoutineId", "Desc", schedule.RoutineId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "Classid", "Classname", schedule.ClassId);
            return View(schedule);
        }

        // GET: Schedules/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Schedules == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule == null)
            {
                return NotFound();
            }
            ViewData["PlanId"] = new SelectList(_context.Paidplans, "PlanId", "PlanTitle", schedule.PlanId);
            ViewData["RoutineId"] = new SelectList(_context.Routines, "RoutineId", "Desc", schedule.RoutineId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "Classid", "Classname", schedule.ClassId);
            return View(schedule);
        }

        // POST: Schedules/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("ScheduleId,Day,Time,ScheduleImageFile,PlanId,RoutineId,ClassId,ImagePath")] Schedule schedule)
        {
            if (id != schedule.ScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (schedule.ScheduleImageFile != null)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(schedule.ScheduleImageFile.FileName);
                        string filePath = Path.Combine(_hostEnvironment.WebRootPath, "Images", fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await schedule.ScheduleImageFile.CopyToAsync(fileStream);
                        }

                        schedule.ImagePath = fileName; // Update the file name in the database
                    }

                    _context.Update(schedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ScheduleExists(schedule.ScheduleId))
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
            ViewData["PlanId"] = new SelectList(_context.Paidplans, "PlanId", "PlanTitle", schedule.PlanId);
            ViewData["RoutineId"] = new SelectList(_context.Routines, "RoutineId", "Desc", schedule.RoutineId);
            ViewData["ClassId"] = new SelectList(_context.Classes, "Classid", "Classname", schedule.ClassId);
            return View(schedule);
        }

        // GET: Schedules/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Schedules == null)
            {
                return NotFound();
            }

            var schedule = await _context.Schedules
                .Include(s => s.Plan)
                .Include(s => s.Routine)
                .Include(s => s.Class)
                .FirstOrDefaultAsync(m => m.ScheduleId == id);
            if (schedule == null)
            {
                return NotFound();
            }

            return View(schedule);
        }

        // POST: Schedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Schedules == null)
            {
                return Problem("Entity set 'ModelContext.Schedules' is null.");
            }
            var schedule = await _context.Schedules.FindAsync(id);
            if (schedule != null)
            {
                _context.Schedules.Remove(schedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ScheduleExists(decimal id)
        {
            return (_context.Schedules?.Any(e => e.ScheduleId == id)).GetValueOrDefault();
        }
    }
}
