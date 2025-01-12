using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheLearningHub_Fitness_Center_Management.Models;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    public class HomePageContentsController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ModelContext _context;

        public HomePageContentsController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }


        // GET: HomePageContents
        public async Task<IActionResult> Index()

        {
              return _context.HomePageContents != null ? 
                          View(await _context.HomePageContents.ToListAsync()) :
                          Problem("Entity set 'ModelContext.HomePageContents'  is null.");
        }

        // GET: HomePageContents/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.HomePageContents == null)
            {
                return NotFound();
            }

            var homePageContent = await _context.HomePageContents
                .FirstOrDefaultAsync(m => m.HomePageId == id);
            if (homePageContent == null)
            {
                return NotFound();
            }

            return View(homePageContent);
        }

        // GET: HomePageContents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HomePageContents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HomePageId,MainTitle,BackgroundTitle1,BackgroundDesc1,BackgroundImageFile1,CourselTextTitle,CourselDesc,CourselItemsTitle,CourselItemsDesc,BackgroundTitle2,BackgroundDesc2,BackgroundImageFile2,TrainerTitle,TrainerDesc,ServicesTitle,ServicesDesc1,ServicesImageFile2,ServicesDesc2,FooterTitle,FooterTitleDesc,FooterEmail")] HomePageContent homePageContent)
        {

            if (ModelState.IsValid)
            {

                if (homePageContent.BackgroundImageFile1 !=null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString()+ '_' + homePageContent.BackgroundImageFile1.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await homePageContent.BackgroundImageFile1.CopyToAsync(fileStream);
                    }
                    homePageContent.BackgroundImagePath1 = fileName;

                }
                if (homePageContent.BackgroundImageFile2 != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + '_' + homePageContent.BackgroundImageFile2.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await homePageContent.BackgroundImageFile2.CopyToAsync(fileStream);
                    }
                    homePageContent.BackgroundImagePath2 = fileName;

                }
                if (homePageContent.ServicesImageFile2 != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + '_' + homePageContent.ServicesImageFile2.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await homePageContent.ServicesImageFile2.CopyToAsync(fileStream);
                    }
                    homePageContent.ServicesImagePath2 = fileName;

                }

                _context.Add(homePageContent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(homePageContent);
        }

        // GET: HomePageContents/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.HomePageContents == null)
            {
                return NotFound();
            }

            var homePageContent = await _context.HomePageContents.FindAsync(id);
            if (homePageContent == null)
            {
                return NotFound();
            }
            return View(homePageContent);
        }

        // POST: HomePageContents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("HomePageId,MainTitle,BackgroundTitle1,BackgroundDesc1,BackgroundImageFile1,CourselTextTitle,CourselDesc,CourselItemsTitle,CourselItemsDesc,BackgroundTitle2,BackgroundDesc2,BackgroundImageFile2,TrainerTitle,TrainerDesc,ServicesTitle,ServicesDesc1,ServicesImageFile2,ServicesDesc2,FooterTitle,FooterTitleDesc,FooterEmail")] HomePageContent homePageContent)
        {
            if (id != homePageContent.HomePageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                if (homePageContent.BackgroundImageFile1 != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + '_' + homePageContent.BackgroundImageFile1.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await homePageContent.BackgroundImageFile1.CopyToAsync(fileStream);
                    }
                    homePageContent.BackgroundImagePath1 = fileName;

                }
                if (homePageContent.BackgroundImageFile2 != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + '_' + homePageContent.BackgroundImageFile2.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await homePageContent.BackgroundImageFile2.CopyToAsync(fileStream);
                    }
                    homePageContent.BackgroundImagePath2 = fileName;

                }
                if (homePageContent.ServicesImageFile2 != null)
                {
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + '_' + homePageContent.ServicesImageFile2.FileName;
                    string path = Path.Combine(wwwRootPath + "/Images/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await homePageContent.ServicesImageFile2.CopyToAsync(fileStream);
                    }
                    homePageContent.ServicesImagePath2 = fileName;

                }
                try
                {
                    _context.Update(homePageContent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HomePageContentExists(homePageContent.HomePageId))
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
            return View(homePageContent);
        }

        // GET: HomePageContents/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.HomePageContents == null)
            {
                return NotFound();
            }

            var homePageContent = await _context.HomePageContents
                .FirstOrDefaultAsync(m => m.HomePageId == id);
            if (homePageContent == null)
            {
                return NotFound();
            }

            return View(homePageContent);
        }

        // POST: HomePageContents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.HomePageContents == null)
            {
                return Problem("Entity set 'ModelContext.HomePageContents'  is null.");
            }
            var homePageContent = await _context.HomePageContents.FindAsync(id);
            if (homePageContent != null)
            {
                _context.HomePageContents.Remove(homePageContent);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HomePageContentExists(decimal id)
        {
          return (_context.HomePageContents?.Any(e => e.HomePageId == id)).GetValueOrDefault();
        }
    }
}
