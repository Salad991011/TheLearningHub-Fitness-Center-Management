using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Mail;
using System.Text;
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

            ViewBag.ActiveSubscription = activeSubscription != null;
            ViewBag.RemainingDays = activeSubscription != null ? (activeSubscription.DateTo.Value - DateTime.Now).Days : 0;
            ViewBag.CurrentEndDate = activeSubscription?.DateTo?.ToString("MM/dd/yyyy");
            ViewBag.ErrorMessage = activeSubscription != null ? "You already have this plan subscribed." : null;

            // Calculate subscription start and end dates for a new subscription
            var startDate = DateTime.Now;
            var endDate = startDate.AddMonths(duration);

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

            // Simulate payment gateway processing
            bool paymentSuccessful = true; // Replace this with real payment gateway logic

            if (paymentSuccessful)
            {
                if (!extend)
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
                }

                // Send Invoice
                await SendInvoiceEmail(userId.Value, plan, duration);

                TempData["SuccessMessage"] = "Payment successful! Subscription activated. An invoice has been sent to your email.";
                return RedirectToAction("PaymentMethod", new { planId, duration, price = plan.PlanPrice });
            }
            else
            {
                TempData["Error"] = "Payment failed. Please try again.";
                return RedirectToAction("PaymentMethod", new { planId, duration, price = plan.PlanPrice });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendInvoice(int planId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var plan = await _context.Paidplans.FirstOrDefaultAsync(p => p.PlanId == planId);
            if (plan == null)
            {
                TempData["Error"] = "Plan not found.";
                return RedirectToAction("Index", "Home");
            }

            await SendInvoiceEmail(userId.Value, plan);

            TempData["SuccessMessage"] = "Invoice sent to your email.";
            return RedirectToAction("PaymentMethod", new { planId, price = plan.PlanPrice });
        }

        private async Task SendInvoiceEmail(int userId, Paidplan plan, int duration = 0)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return;
            }

            var invoiceHtml = new StringBuilder();
            invoiceHtml.AppendLine("<h1>Invoice</h1>");
            invoiceHtml.AppendLine($"<p>Plan: {plan.PlanTitle}</p>");
            invoiceHtml.AppendLine($"<p>Price: ${plan.PlanPrice}</p>");
            if (duration > 0)
            {
                invoiceHtml.AppendLine($"<p>Duration: {duration} months</p>");
                invoiceHtml.AppendLine($"<p>Start Date: {DateTime.Now:MM/dd/yyyy}</p>");
                invoiceHtml.AppendLine($"<p>End Date: {DateTime.Now.AddMonths(duration):MM/dd/yyyy}</p>");
            }
            invoiceHtml.AppendLine("<p>Thank you for your purchase!</p>");

            using (var message = new MailMessage())
            {
                message.From = new MailAddress("Saladforyra@Saladsc.onmicrosoft.com", "Workout Team"); // Set a valid "From" address
                message.To.Add(new MailAddress(user.Email));
                message.Subject = "Your Invoice";
                message.Body = invoiceHtml.ToString();
                message.IsBodyHtml = true;

                using (var smtpClient = new SmtpClient("smtp.office365.com", 587)) // Replace with your SMTP server and port
                {
                    smtpClient.Credentials = new System.Net.NetworkCredential("Saladforyra@Saladsc.onmicrosoft.com", "Salad1234"); // Replace with your email credentials
                    smtpClient.EnableSsl = true;
                    await smtpClient.SendMailAsync(message);
                }
            }
        }

    }
}
