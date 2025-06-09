using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chill_Closet.Controllers
{
    [Authorize(Roles = "Admin")] // <-- CHỈ ĐỊNH RẰNG CHỈ CÓ ADMIN MỚI ĐƯỢC VÀO
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}