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
    public class ServicesController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ServicesController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Services
        public async Task<IActionResult> Index()
        {
            if (_context.Services == null)
                return Problem("Entity set 'ModelContext.Services' is null.");

            var services = await _context.Services.ToListAsync();
            return View(services);
        }

        // GET: Services/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (!id.HasValue)
                return NotFound();

            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == id);

            return service == null ? NotFound() : View(service);
        }

        // GET: Services/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Services/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ServiceId,ServiceName,ServiceDesc,ServicesImageFile")] Service service)
        {
            if (!ModelState.IsValid)
                return View(service);

            if (service.ServicesImageFile != null)
                service.ImagePath = await SaveImageFile(service.ServicesImageFile);

            _context.Add(service);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Services/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (!id.HasValue)
                return NotFound();

            var service = await _context.Services.FindAsync(id);
            return service == null ? NotFound() : View(service);
        }

        // POST: Services/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("ServiceId,ServiceName,ServiceDesc,ServicesImageFile")] Service service)
        {
            if (id != service.ServiceId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(service);

            if (service.ServicesImageFile != null)
                service.ImagePath = await SaveImageFile(service.ServicesImageFile);

            try
            {
                _context.Update(service);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(service.ServiceId))
                    return NotFound();

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Services/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (!id.HasValue)
                return NotFound();

            var service = await _context.Services.FirstOrDefaultAsync(s => s.ServiceId == id);

            return service == null ? NotFound() : View(service);
        }

        // POST: Services/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service != null)
            {
                _context.Services.Remove(service);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ServiceExists(decimal id)
        {
            return _context.Services?.Any(e => e.ServiceId == id) ?? false;
        }

        private async Task<string> SaveImageFile(IFormFile imageFile)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = $"{Guid.NewGuid()}_{imageFile.FileName}";
            string path = Path.Combine(wwwRootPath, "Images", fileName);

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return fileName;
        }
    }
}
