using Microsoft.AspNetCore.Mvc;

namespace Chill_Closet.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}