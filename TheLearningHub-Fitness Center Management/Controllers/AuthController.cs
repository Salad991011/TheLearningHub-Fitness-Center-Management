using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using TheLearningHub_Fitness_Center_Management.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
	public class AuthController : Controller
	{
		private readonly ModelContext _context;

		public AuthController(ModelContext context)
		{
			_context = context;
		}

		// Display the Login View
		public IActionResult Login(string returnUrl = "/")
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View("~/Views/LoginAndRegister/Login.cshtml");

		}

		// Handle Login POST Request
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(string username, string password, string returnUrl = "/")
		{
			var user = await _context.Logins
				.FirstOrDefaultAsync(l => l.UserName == username && l.Password == password);

			if (user != null)
			{
				// Set session variables
				HttpContext.Session.SetString("Username", user.UserName);
				HttpContext.Session.SetInt32("RoleId", (int)user.RoleId.GetValueOrDefault());
				HttpContext.Session.SetInt32("LoginId", (int)user.LoginId);

				// Sign in the user
				var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.UserName),
			new Claim(ClaimTypes.Role, user.RoleId.ToString()) // Optional: Include role if needed
        };

				var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
				await HttpContext.SignInAsync("Cookies", new ClaimsPrincipal(claimsIdentity));

				TempData["Message"] = "Login successful!";
				return LocalRedirect(returnUrl); // Redirect to the originally requested page
			}

			TempData["Error"] = "Invalid username or password.";
			ViewData["ReturnUrl"] = returnUrl;
			return View("~/Views/LoginAndRegister/Login.cshtml");
		}


		// Display the Register View
		public IActionResult Register()
		{
			return View("~/Views/LoginAndRegister/Register.cshtml");
		}

		// Handle Register POST Request
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(string email, string password, string confirm_password)
		{
			if (password != confirm_password)
			{
				TempData["Error"] = "Passwords do not match.";
				return View("~/Views/LoginAndRegister/Register.cshtml");
			}

			// Check if username already exists
			var existingUser = await _context.Logins.FirstOrDefaultAsync(l => l.UserName == email);
			if (existingUser != null)
			{
				TempData["Error"] = "This email is already registered.";
				return View("~/Views/LoginAndRegister/Register.cshtml");
			}

			// Create a new Login entry
			var newLogin = new Login
			{
				UserName = email,
				Password = password,
				RoleId = 2 // Default role (e.g., User role)
			};

			_context.Logins.Add(newLogin);
			await _context.SaveChangesAsync();

			TempData["Message"] = "Registration successful! Please log in.";
			return RedirectToAction("Login");
		}

		// Handle Logout
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync("Cookies"); // Clear the authentication cookie
			HttpContext.Session.Clear(); // Clear session variables
			return RedirectToAction("Login");
		}

	}
}
