using Chill_Closet.Data;
using Chill_Closet.Enums;
using Chill_Closet.Infrastructure;
using Chill_Closet.Models;
using Chill_Closet.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chill_Closet.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrderRepository _orderRepository;
        private readonly IVoucherRepository _voucherRepository; // Thêm repository mới

        // Cập nhật constructor để inject cả 3 repository
        public CheckoutController(UserManager<ApplicationUser> userManager,
                                  IOrderRepository orderRepository,
                                  IVoucherRepository voucherRepository)
        {
            _userManager = userManager;
            _orderRepository = orderRepository;
            _voucherRepository = voucherRepository;
        }

        // Action GET Index giữ nguyên
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ShoppingCart cart = HttpContext.Session.GetJson<ShoppingCart>("Cart") ?? new ShoppingCart();

            if (cart.Items.Count == 0)
            {
                TempData["Message"] = "Giỏ hàng của bạn đang trống!";
                return RedirectToAction("Index", "ShoppingCart");
            }

            var viewModel = new CheckoutViewModel
            {
                FullName = user.FullName,
                ShippingAddress = user.Address,
                ShippingPhone = user.PhoneNumber,
                Email = user.Email
            };

            ViewBag.Cart = cart;
            return View(viewModel);
        }

        // Action POST Index được cập nhật để xử lý voucher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(CheckoutViewModel viewModel)
        {
            ShoppingCart cart = HttpContext.Session.GetJson<ShoppingCart>("Cart") ?? new ShoppingCart();
            if (cart.Items.Count == 0)
            {
                ModelState.AddModelError("", "Giỏ hàng của bạn đang trống!");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var order = new Order
                {
                    FullName = viewModel.FullName,
                    ShippingAddress = viewModel.ShippingAddress,
                    ShippingPhone = viewModel.ShippingPhone,
                    Email = viewModel.Email,
                    Notes = viewModel.Notes,
                    ApplicationUserId = user.Id,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Pending
                };

                // Lấy mã voucher từ session
                var appliedVoucherCode = HttpContext.Session.GetString("AppliedVoucher");
                if (!string.IsNullOrEmpty(appliedVoucherCode))
                {
                    var voucher = await _voucherRepository.GetByCodeAsync(appliedVoucherCode);
                    if (voucher != null)
                    {
                        // Tính lại tổng tiền cuối cùng có áp dụng voucher
                        order.TotalAmount = cart.GetGrandTotal(voucher);

                        // Giảm số lượng voucher và cập nhật
                        voucher.Quantity -= 1;
                        await _voucherRepository.UpdateAsync(voucher);
                    }
                }
                else
                {
                    // Nếu không có voucher, lấy tổng tiền bình thường
                    order.TotalAmount = cart.GetGrandTotal();
                }

                foreach (var item in cart.Items)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = item.Price
                    });
                }

                await _orderRepository.CreateOrderAsync(order);

                // Xóa cả giỏ hàng và voucher đã áp dụng khỏi session
                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove("AppliedVoucher");

                return View("OrderCompleted", order);
            }

            ViewBag.Cart = cart;
            return View(viewModel);
        }

        public IActionResult OrderCompleted(Order order)
        {
            // Hiển thị thông báo cảm ơn với mã đơn hàng
            ViewBag.OrderNumber = order.Id;
            return View();
        }
    }
}