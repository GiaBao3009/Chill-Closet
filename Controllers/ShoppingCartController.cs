using Microsoft.AspNetCore.Mvc;

namespace Chill_Closet.Controllers
{
    public class ShoppingCartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
