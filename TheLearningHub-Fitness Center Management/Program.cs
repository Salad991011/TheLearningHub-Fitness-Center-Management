using Microsoft.EntityFrameworkCore;
using TheLearningHub_Fitness_Center_Management.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ModelContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("First Project")));
builder.Services.AddSession(options => {
    options.IdleTimeout = TimeSpan.FromMinutes(60);
});
builder.Services.AddAuthentication("Cookies")
	.AddCookie(options =>
	{
		options.LoginPath = "/Auth/Login"; // Set the login path to the AuthController's Login action
		options.AccessDeniedPath = "/Auth/AccessDenied"; // Optional: Handle access denied cases
	});
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(60); // Set session timeout to 60 minutes
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();         // Ensure session is set up here
app.UseAuthentication();  // Enable authentication middleware
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
