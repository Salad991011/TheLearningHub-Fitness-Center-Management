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
            var paidPlans = await _context.Paidplans.Include(p => p.User).ToListAsync();
            return View(paidPlans);
        }

        // GET: Paidplans/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
                return NotFound();

            var paidPlan = await _context.Paidplans
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PlanId == id);

            if (paidPlan == null)
                return NotFound();

            return View(paidPlan);
        }

        // GET: Paidplans/Create
        public IActionResult Create()
        {
            PopulateUsersDropdown();
            return View();
        }

        // POST: Paidplans/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlanId,PlanTitle,PlanPrice,PlanDesc,PlanImageFile,UserId")] Paidplan paidPlan)
        {
            if (ModelState.IsValid)
            {
                if (paidPlan.PlanImageFile != null)
                {
                    paidPlan.ImagePath = SaveImage(paidPlan.PlanImageFile);
                }

                _context.Add(paidPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            PopulateUsersDropdown(paidPlan.UserId);
            return View(paidPlan);
        }

        // GET: Paidplans/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
                return NotFound();

            var paidPlan = await _context.Paidplans.FindAsync(id);
            if (paidPlan == null)
                return NotFound();

            PopulateUsersDropdown(paidPlan.UserId);
            return View(paidPlan);
        }

        // POST: Paidplans/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("PlanId,PlanTitle,PlanPrice,PlanDesc,PlanImageFile,UserId")] Paidplan paidPlan)
        {
            if (id != paidPlan.PlanId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (paidPlan.PlanImageFile != null)
                    {
                        paidPlan.ImagePath = SaveImage(paidPlan.PlanImageFile);
                    }

                    _context.Update(paidPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaidplanExists(paidPlan.PlanId))
                        return NotFound();

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            PopulateUsersDropdown(paidPlan.UserId);
            return View(paidPlan);
        }

        // GET: Paidplans/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
                return NotFound();

            var paidPlan = await _context.Paidplans
                .Include(p => p.User)
                .FirstOrDefaultAsync(m => m.PlanId == id);

            if (paidPlan == null)
                return NotFound();

            return View(paidPlan);
        }

        // POST: Paidplans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var paidPlan = await _context.Paidplans.FindAsync(id);

            if (paidPlan != null)
            {
                _context.Paidplans.Remove(paidPlan);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PaidplanExists(decimal id)
        {
            return _context.Paidplans.Any(e => e.PlanId == id);
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

        // Helper Method to Populate Users Dropdown
        private void PopulateUsersDropdown(decimal? selectedUserId = null)
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId", selectedUserId);
        }
    }
}
