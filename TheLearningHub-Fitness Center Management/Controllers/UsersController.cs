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
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,Fname,Lname,Email,PhoneNumber,UsersImageFile,RoleId")] User user, string password)
        {
            if (!ModelState.IsValid)
            {
                PopulateRoleDropDownList(user.RoleId);
                return View(user);
            }

            // Check if the email already exists in Users or Logins
            if (await _context.Users.AnyAsync(u => u.Email == user.Email) || await _context.Logins.AnyAsync(l => l.UserName == user.Email))
            {
                TempData["Error"] = "A user with this email already exists.";
                PopulateRoleDropDownList(user.RoleId);
                return View(user);
            }

            // Handle image upload
            if (user.UsersImageFile != null)
                user.ImagePath = await SaveImageFile(user.UsersImageFile);

            // Create a new Login record for this user
            var login = new Login
            {
                UserName = user.Email, // Use email as username
                Password = password,   // Use plain password as requested
                RoleId = user.RoleId   // Assign the correct role
            };

            _context.Logins.Add(login);
            await _context.SaveChangesAsync();

            // Assign the newly created LoginId to the user
            user.LoginId = login.LoginId;

            // Save the new User record
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "User created successfully!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (!id.HasValue)
                return NotFound();

            var user = await _context.Users.Include(u => u.Login).FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                return NotFound();

            PopulateRoleDropDownList(user.RoleId);
            return View(user);
        }
        private bool UserExists(decimal id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("UserId,Fname,Lname,Email,PhoneNumber,UsersImageFile,RoleId")] User user)
        {
            if (id != user.UserId)
                return NotFound();

            if (!ModelState.IsValid)
            {
                PopulateRoleDropDownList(user.RoleId);
                return View(user);
            }

            // Handle image upload
            if (user.UsersImageFile != null)
                user.ImagePath = await SaveImageFile(user.UsersImageFile);

            try
            {
                // Update user details
                _context.Update(user);
                await _context.SaveChangesAsync();
                TempData["Success"] = "User updated successfully!";
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
        public async Task<IActionResult> DeleteConfirmed(decimal id, string password)
        {
            // Retrieve the logged-in user's LoginId from the session
            var loginId = HttpContext.Session.GetInt32("LoginId");

            if (loginId == null)
            {
                TempData["Error"] = "You must be logged in to perform this action.";
                return RedirectToAction("Index");
            }

            // Fetch the logged-in user's Login record
            var loggedInUser = await _context.Logins.FirstOrDefaultAsync(l => l.LoginId == loginId);

            // Validate the entered password against the logged-in user's password
            if (loggedInUser == null || loggedInUser.Password != password)
            {
                TempData["Error"] = "Invalid password. Please try again.";
                return RedirectToAction("Delete", new { id });
            }

            // Fetch the user to be deleted
            var user = await _context.Users
                .Include(u => u.Login)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            try
            {
                // Delete the user's associated Login record, if it exists
                if (user.Login != null)
                {
                    _context.Logins.Remove(user.Login);
                }

                // Delete the user record
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                TempData["Success"] = "User deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while deleting the user. Please try again.";
                Console.WriteLine($"Error: {ex.Message}");
                return RedirectToAction("Delete", new { id });
            }
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
    }
}
