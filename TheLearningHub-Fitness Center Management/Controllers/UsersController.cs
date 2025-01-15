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
    public class UsersController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ModelContext _context;

        public UsersController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.Include(u => u.Login).Include(u => u.Role).ToListAsync();
            return View(users);
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (!id.HasValue)
                return NotFound();

            var user = await _context.Users
                .Include(u => u.Login)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            return user == null ? NotFound() : View(user);
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            PopulateRoleDropDownList();
            PopulateLoginDropDownList();
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Fname,Lname,Email,PhoneNumber,UsersImageFile,LoginId,RoleId")] User user)
        {
            if (!ModelState.IsValid)
            {
                PopulateRoleDropDownList(user.RoleId);
                PopulateLoginDropDownList(user.LoginId);
                return View(user);
            }

            if (!_context.Roles.Any(r => r.RoleId == user.RoleId))
            {
                ModelState.AddModelError("RoleId", "Selected role does not exist.");
                PopulateRoleDropDownList(user.RoleId);
                PopulateLoginDropDownList(user.LoginId);
                return View(user);
            }

            if (user.UsersImageFile != null)
                user.ImagePath = await SaveImageFile(user.UsersImageFile);

            try
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error: {ex.InnerException?.Message}");
                ModelState.AddModelError("", "An error occurred while saving the user. Please try again.");
                PopulateRoleDropDownList(user.RoleId);
                PopulateLoginDropDownList(user.LoginId);
                return View(user);
            }
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (!id.HasValue)
                return NotFound();

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            PopulateRoleDropDownList(user.RoleId);
            PopulateLoginDropDownList(user.LoginId);
            return View(user);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("UserId,Fname,Lname,Email,PhoneNumber,UsersImageFile,LoginId,RoleId")] User user)
        {
            if (id != user.UserId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateRoleDropDownList(user.RoleId);
                PopulateLoginDropDownList(user.LoginId);
                return View(user);
            }

            if (user.UsersImageFile != null)
                user.ImagePath = await SaveImageFile(user.UsersImageFile);

            try
            {
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(user.UserId))
                    return NotFound();

                throw;
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error: {ex.InnerException?.Message}");
                ModelState.AddModelError("", "An error occurred while updating the user. Please try again.");
                PopulateRoleDropDownList(user.RoleId);
                PopulateLoginDropDownList(user.LoginId);
                return View(user);
            }
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (!id.HasValue)
                return NotFound();

            var user = await _context.Users
                .Include(u => u.Login)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            return user == null ? NotFound() : View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(decimal id)
        {
            return _context.Users?.Any(e => e.UserId == id) ?? false;
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

        private void PopulateRoleDropDownList(object selectedRole = null)
        {
            ViewBag.Roles = new SelectList(_context.Roles, "RoleId", "RoleName", selectedRole);
        }

        private void PopulateLoginDropDownList(object selectedLogin = null)
        {
            var logins = _context.Logins.Select(l => new
            {
                l.LoginId,
                Display = $"{l.LoginId} - {l.UserName}"
            });

            ViewData["LoginId"] = new SelectList(logins, "LoginId", "Display", selectedLogin);
        }
        public IActionResult AssignedUsers()
        {
            var trainerId = HttpContext.Session.GetInt32("TrainerId");
            var users = _context.Users
                .Include(u => u.Paidplans)
                .Where(u => u.Paidplans.Any(p => p.TrainerId == trainerId))
                .ToList();

            return View(users);
        }

    }
}
