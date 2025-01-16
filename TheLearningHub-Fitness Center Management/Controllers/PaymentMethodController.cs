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
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var plan = _context.Paidplans.FirstOrDefault(p => p.PlanId == planId);
            if (plan == null)
            {
                return NotFound("Plan not found.");
            }

            // Check for an active subscription
            var activeSubscription = _context.Subscriptions
                .FirstOrDefault(s => s.PlanId == planId && s.UserId == userId && s.DateTo > DateTime.Now);

            if (activeSubscription != null)
            {
                // Notify about active subscription and calculate remaining duration
                var remainingDays = (activeSubscription.DateTo.Value - DateTime.Now).Days;
                ViewBag.ActiveSubscription = true;
                ViewBag.RemainingDays = remainingDays;
                ViewBag.CurrentEndDate = activeSubscription.DateTo.Value.ToString("MM/dd/yyyy");
            }
            else
            {
                ViewBag.ActiveSubscription = false;
            }

            // Calculate subscription start and end dates for new subscription
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
        public async Task<IActionResult> ProcessPayment(int planId, int duration, string cardNumber, string cardHolderName, string expiryDate, string cvv, bool extend = false)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var plan = await _context.Paidplans.FirstOrDefaultAsync(p => p.PlanId == planId);
            if (plan == null)
            {
                TempData["Error"] = "Invalid plan.";
                return RedirectToAction("Index", "Home");
            }

            // Validate payment details
            if (string.IsNullOrWhiteSpace(cardNumber) || string.IsNullOrWhiteSpace(cardHolderName) ||
                string.IsNullOrWhiteSpace(expiryDate) || string.IsNullOrWhiteSpace(cvv))
            {
                TempData["Error"] = "Please provide all payment details.";
                return RedirectToAction("PaymentMethod", new { planId, duration, price = plan.PlanPrice });
            }

            // Simulate payment gateway processing
            bool paymentSuccessful = true; // Replace this with real payment gateway logic

            if (paymentSuccessful)
            {
                if (extend)
                {
                    // Extend the active subscription
                    var activeSubscription = await _context.Subscriptions
                        .FirstOrDefaultAsync(s => s.PlanId == planId && s.UserId == userId && s.DateTo > DateTime.Now);

                    if (activeSubscription != null)
                    {
                        activeSubscription.DateTo = activeSubscription.DateTo.Value.AddMonths(duration);
                        _context.Subscriptions.Update(activeSubscription);
                        await _context.SaveChangesAsync();

                        TempData["SuccessMessage"] = $"Subscription extended successfully! New end date: {activeSubscription.DateTo:MM/dd/yyyy}";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    // Create a new subscription
                    var startDate = DateTime.Now;
                    var endDate = startDate.AddMonths(duration);

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
            }
            else
            {
                TempData["Error"] = "Payment failed. Please try again.";
                return RedirectToAction("PaymentMethod", new { planId, duration, price = plan.PlanPrice });
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
