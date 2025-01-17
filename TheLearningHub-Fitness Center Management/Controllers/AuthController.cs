using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using TheLearningHub_Fitness_Center_Management.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    public class AuthController : Controller
    {
        private readonly ModelContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AuthController(ModelContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        public IActionResult Login(string returnUrl = "/")
        {


            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Views/LoginAndRegister/Login.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string returnUrl = "/")
        {
            // Log inputs
            Console.WriteLine($"Login Attempt: Username={username}, Password={password}");

            // Fetch user from Logins and Users tables
            var user = await _context.Logins
                .Join(
                    _context.Users,
                    login => login.LoginId,
                    user => user.LoginId,
                    (login, user) => new { Login = login, User = user }
                )
                .FirstOrDefaultAsync(joined =>
                    (joined.Login.UserName.ToLower() == username.ToLower() || joined.User.Email.ToLower() == username.ToLower()) &&
                    joined.Login.Password == password
                );

            if (user == null)
            {
                TempData["Error"] = "Invalid username or password.";
                return View("~/Views/LoginAndRegister/Login.cshtml");
            }

            // Store session data
            HttpContext.Session.SetInt32("LoginId", (int)user.Login.LoginId);
            HttpContext.Session.SetInt32("RoleId", (int)user.Login.RoleId.GetValueOrDefault());
            HttpContext.Session.SetInt32("UserId", (int)user.User.UserId);
            HttpContext.Session.SetString("Username", user.Login.UserName ?? user.User.Email);

            // Assuming `user.ProfileImagePath` contains the file name or relative path
           
            // Set up claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Login.UserName ?? user.User.Email),
        new Claim(ClaimTypes.Role, user.Login.RoleId.ToString())
    };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
            await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

            // Redirect based on role
            switch (user.Login.RoleId)
            {
                case 1: // Admin
                    return RedirectToAction("AdminDashboard", "AdminDashboard");
                case 3: // Trainer
                    return RedirectToAction("TrainerDashboard", "TrainerDashboard");
                case 2: // User
                    return RedirectToAction("Index", "Home");
                default:
                    TempData["Error"] = "Invalid role.";
                    return View("~/Views/LoginAndRegister/Login.cshtml");
            }
        }




        public IActionResult Register()
        {
            return View("~/Views/LoginAndRegister/Register.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string confirm_password, string fname, string lname, string phoneNumber, IFormFile profileImage)
        {
            if (password != confirm_password)
            {
                TempData["Error"] = "Passwords do not match.";
                return View("~/Views/LoginAndRegister/Register.cshtml");
            }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                TempData["Error"] = "This email is already registered.";
                return View("~/Views/LoginAndRegister/Register.cshtml");
            }

            // Handle image upload
            string imagePath = null;
            if (profileImage != null)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string fileName = $"{Guid.NewGuid()}_{profileImage.FileName}";
                string path = Path.Combine(wwwRootPath, "Images", fileName);

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await profileImage.CopyToAsync(fileStream);
                }
                imagePath = fileName;
            }

            // Create a Login record for the registered user
            var newLogin = new Login
            {
                UserName = email,
                Password = password,
                RoleId = 2 // Default role for regular users
            };

            _context.Logins.Add(newLogin);
            await _context.SaveChangesAsync();

            // Create a User record linked to the Login record
            var newUser = new User
            {
                Fname = fname,
                Lname = lname,
                Email = email,
                PhoneNumber = phoneNumber,
                ImagePath = imagePath,
                LoginId = newLogin.LoginId,
                RoleId = 2 // Default role for regular users
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Registration successful! Please log in.";
            return RedirectToAction("Login");
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
