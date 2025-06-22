using Chill_Closet.Infrastructure;
using Chill_Closet.Models;
using Chill_Closet.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Chill_Closet.Controllers
{
    public class ShoppingCartController : Controller
    {
       private readonly IProductRepository _productRepository;
        private readonly IVoucherRepository _voucherRepository;

        public ShoppingCartController(IProductRepository productRepository, IVoucherRepository voucherRepository)
        {
            _productRepository = productRepository;
            _voucherRepository = voucherRepository;
        }

        public IActionResult Index()
        {
            ShoppingCart cart = HttpContext.Session.GetJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            var appliedVoucherCode = HttpContext.Session.GetString("AppliedVoucher");

            var viewModel = new ShoppingCartViewModel
            {
                ShoppingCart = cart,
                Subtotal = cart.GetSubtotal(),
                ShippingFee = cart.GetShippingFee(),
                GrandTotal = cart.GetGrandTotal(),
                AppliedVoucherCode = appliedVoucherCode // Truyền mã voucher đã áp dụng sang View
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            Product product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại!" });
            }

            ShoppingCart cart = HttpContext.Session.GetJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.AddItem(new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                Quantity = quantity,
                ImageUrl = product.MainImageUrl
            });
            HttpContext.Session.SetJson("Cart", cart);

            return Json(new
            {
                success = true,
                message = "Đã thêm sản phẩm vào giỏ hàng!",
                cartCount = cart.Items.Sum(i => i.Quantity),
                grandTotal = cart.GetGrandTotal().ToString("C", new System.Globalization.CultureInfo("vi-VN"))
            });
        }

        public IActionResult RemoveFromCart(int productId)
        {
            ShoppingCart cart = HttpContext.Session.GetJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            cart.RemoveItem(productId);
            HttpContext.Session.SetJson("Cart", cart);
            return RedirectToAction("Index");
        }

        // === ACTION BỊ THIẾU NẰM Ở ĐÂY ===
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyVoucher(string voucherCode)
        {
            var voucher = await _voucherRepository.GetByCodeAsync(voucherCode);
            var cart = HttpContext.Session.GetJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            if (voucher == null)
            {
                HttpContext.Session.Remove("AppliedVoucher");
                return Json(new { success = false, message = "Mã voucher không hợp lệ hoặc đã hết hạn." });
            }

            HttpContext.Session.SetString("AppliedVoucher", voucher.Code);
            var grandTotal = cart.GetGrandTotal(voucher);
            var discountAmount = cart.GetSubtotal() - (grandTotal - cart.GetShippingFee());

            return Json(new
            {
                success = true,
                message = $"Áp dụng voucher '{voucher.Code}' thành công!",
                newTotal = grandTotal,
                discountAmount = discountAmount
            });
        }
    }
}