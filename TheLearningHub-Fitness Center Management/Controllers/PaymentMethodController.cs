using Microsoft.AspNetCore.Mvc;

namespace TheLearningHub_Fitness_Center_Management.Controllers
{
    public class PaymentMethodController : Controller
    {
        public IActionResult PaymentMethod()
        {
            return View();
        }
    }
}
