using Microsoft.AspNetCore.Mvc;

namespace Chill_Closet.Controllers
{
    public class BlogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // THÊM ACTION MỚI DƯỚI ĐÂY
        // GET: /Blog/Details/1 (ví dụ với id=1)
        public IActionResult Details(int id)
        {
            // Sau này, bạn sẽ dùng 'id' để lấy bài viết cụ thể từ database
            return View();
        }
    }
}