using Microsoft.AspNetCore.Mvc;

namespace Chill_Closet.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Shop/Details/5 (ví dụ với id=5)
        public IActionResult Details(int id)
        {
            // Sau này, bạn sẽ dùng 'id' để truy vấn database và lấy thông tin sản phẩm.
            // Tạm thời chúng ta chỉ hiển thị view.
            return View();
        }
    }
}
