using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using TheLearningHub_Fitness_Center_Management.Models;
using TheLearningHub_Fitness_Center_Management.Middleware;

namespace TheLearningHub_Fitness_Center_Management.Middleware
{
    public class ProfileImageMiddleware
    {
        private readonly RequestDelegate _next;

        public ProfileImageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ModelContext dbContext)
        {
            // Get the LoginId from the session
            var loginId = context.Session.GetInt32("LoginId");
            if (loginId.HasValue)
            {
                // Convert loginId to decimal to match the database primary key type
                var loginIdAsDecimal = Convert.ToDecimal(loginId.Value);

                // Query the database for the user's profile image
                var user = await dbContext.Users.FindAsync(loginIdAsDecimal);
                var profileImagePath = user?.ImagePath != null
                    ? $"~/Images/{user.ImagePath}"
                    : "/AdminDesign/assets/images/faces/placeholder.png";

                // Add the profile image path to the HttpContext.Items
                context.Items["ProfileImage"] = profileImagePath;
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }

}
