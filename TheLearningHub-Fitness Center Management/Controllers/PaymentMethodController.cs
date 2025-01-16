using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheLearningHub_Fitness_Center_Management.Models;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    public class PaymentMethodController : Controller
    {
        private readonly ModelContext _context;

        public PaymentMethodController(ModelContext context)
        {
            _context = context;
        }

        public IActionResult PaymentMethod(int planId, int duration, decimal price)
        {
            var plan = _context.Paidplans.FirstOrDefault(p => p.PlanId == planId);
            if (plan == null)
            {
                return NotFound("Plan not found.");
            }

            // Calculate subscription start and end dates
            var startDate = DateTime.Now;
            var endDate = startDate.AddMonths(duration);

            // Pass data to the view
            ViewBag.StartDate = startDate.ToString("MM/dd/yyyy");
            ViewBag.EndDate = endDate.ToString("MM/dd/yyyy");
            ViewBag.SelectedDuration = duration;
            ViewBag.Price = price;

            return View(plan);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(int planId, int duration, string cardNumber, string cardHolderName, string expiryDate, string cvv)
        {
            // Check if the user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Validate the plan
            var plan = await _context.Paidplans.FirstOrDefaultAsync(p => p.PlanId == planId);
            if (plan == null)
            {
                TempData["Error"] = "Invalid plan.";
                return RedirectToAction("Index", "Home");
            }

            // Validate payment details (basic validation for now)
            if (string.IsNullOrWhiteSpace(cardNumber) || string.IsNullOrWhiteSpace(cardHolderName) ||
                string.IsNullOrWhiteSpace(expiryDate) || string.IsNullOrWhiteSpace(cvv))
            {
                TempData["Error"] = "Please provide all payment details.";
                return RedirectToAction("PaymentMethod", new { planId, duration, price = plan.PlanPrice });
            }

            // Simulate payment gateway processing (e.g., Stripe, PayPal, etc.)
            bool paymentSuccessful = true; // Replace this with real payment gateway logic

            if (paymentSuccessful)
            {
                // Calculate subscription dates
                var startDate = DateTime.Now;
                var endDate = startDate.AddMonths(duration);

                // Create a subscription
                var subscription = new Subscription
                {
                    UserId = userId.Value,
                    PlanId = planId,
                    DateFrom = startDate,
                    DateTo = endDate
                };

                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Payment successful! Subscription activated.";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Error"] = "Payment failed. Please try again.";
                return RedirectToAction("PaymentMethod", new { planId, duration, price = plan.PlanPrice });
            }
        }
    }

}
