using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using TheLearningHub_Fitness_Center_Management.Models;
using Microsoft.AspNetCore.Http;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    public class AuthController : Controller
    {
        private readonly ModelContext _context;

        public AuthController(ModelContext context)
        {
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
            var user = await _context.Logins.FirstOrDefaultAsync(l => l.UserName == username && l.Password == password);

            if (user != null)
            {
                HttpContext.Session.SetInt32("LoginId", (int)user.LoginId); // Store LoginId as an int
                HttpContext.Session.SetInt32("RoleId", (int)user.RoleId.GetValueOrDefault()); // Store RoleId
                HttpContext.Session.SetString("Username", user.UserName); // Store Username

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.RoleId.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
                await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

                switch (user.RoleId)
                {
                    case 1: // Admin
                        return RedirectToAction("AdminDashboard", "AdminDashboard");
                    case 3: // Trainer
                        return RedirectToAction("TrainerDashboard", "Home");
                    case 2: // User
                        return RedirectToAction("Index", "Home");
                    default:
                        TempData["Error"] = "Invalid role.";
                        break;
                }
            }

            TempData["Error"] = "Invalid username or password.";
            ViewData["ReturnUrl"] = returnUrl;
            return View("~/Views/LoginAndRegister/Login.cshtml");
        }

        public IActionResult Register()
        {
            return View("~/Views/LoginAndRegister/Register.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string email, string password, string confirm_password)
        {
            if (password != confirm_password)
            {
                TempData["Error"] = "Passwords do not match.";
                return View("~/Views/LoginAndRegister/Register.cshtml");
            }

            if (await _context.Logins.AnyAsync(l => l.UserName == email))
            {
                TempData["Error"] = "This email is already registered.";
                return View("~/Views/LoginAndRegister/Register.cshtml");
            }

            var newLogin = new Login
            {
                UserName = email,
                Password = password,
                RoleId = 2 // Default to User role
            };

            _context.Logins.Add(newLogin);
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
