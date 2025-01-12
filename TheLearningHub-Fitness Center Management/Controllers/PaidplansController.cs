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
    public class PaidplansController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ModelContext _context;

        public PaidplansController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        // GET: Paidplans
        public async Task<IActionResult> Index()
        {
            var modelContext = _context.Paidplans.Include(p => p.User);
            return View(await modelContext.ToListAsync());
        }

        // GET: Paidplans/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Paidplans == null)
            {
                return NotFound();
            }

            var paidplan = await _context.Paidplans
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PlanId == id);
            if (paidplan == null)
            {
                return NotFound();
            }

            return View(paidplan);
        }

        // GET: Paidplans/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return View();
        }

        // POST: Paidplans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlanId,PlanTitle,PlanPrice,PlanDesc,PlanImageFile,UserId")] Paidplan paidplan)
        {
            if (ModelState.IsValid)
            {
                if (paidplan.PlanImageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + '_' + paidplan.PlanImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await paidplan.PlanImageFile.CopyToAsync(fileStream);
                    }
                    paidplan.ImagePath = fileName;

                }
                _context.Add(paidplan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", paidplan.UserId);
            return View(paidplan);
        }

        // GET: Paidplans/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Paidplans == null)
            {
                return NotFound();
            }

            var paidplan = await _context.Paidplans.FindAsync(id);
            if (paidplan == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", paidplan.UserId);
            return View(paidplan);
        }

        // POST: Paidplans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("PlanId,PlanTitle,PlanPrice,PlanDesc,ImagePath,UserId")] Paidplan paidplan)
        {
            if (id != paidplan.PlanId)
            {

                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (paidplan.PlanImageFile != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + '_' + paidplan.PlanImageFile.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await paidplan.PlanImageFile.CopyToAsync(fileStream);
                    }
                    paidplan.ImagePath = fileName;

                }
                try
                {
                    _context.Update(paidplan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaidplanExists(paidplan.PlanId))
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
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", paidplan.UserId);
            return View(paidplan);
        }

        // GET: Paidplans/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Paidplans == null)
            {
                return NotFound();
            }

            var paidplan = await _context.Paidplans
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PlanId == id);
            if (paidplan == null)
            {
                return NotFound();
            }

            return View(paidplan);
        }

        // POST: Paidplans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Paidplans == null)
            {
                return Problem("Entity set 'ModelContext.Paidplans'  is null.");
            }
            var paidplan = await _context.Paidplans.FindAsync(id);
            if (paidplan != null)
            {
                _context.Paidplans.Remove(paidplan);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaidplanExists(decimal id)
        {
          return (_context.Paidplans?.Any(e => e.PlanId == id)).GetValueOrDefault();
        }
    }
}
