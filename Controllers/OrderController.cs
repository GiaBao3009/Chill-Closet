using Chill_Closet.Enums;
using Chill_Closet.Models;
using Chill_Closet.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Chill_Closet.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVoucherRepository _voucherRepository; // <-- 1. KHAI BÁO THÊM

        public OrderController(IOrderRepository orderRepository,
                               UserManager<ApplicationUser> userManager,
                               IVoucherRepository voucherRepository) // <-- 2. THÊM VÀO CONSTRUCTOR
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _voucherRepository = voucherRepository; // <-- 3. GÁN GIÁ TRỊ
        }

        // MỘT ACTION INDEX DUY NHẤT
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _orderRepository.GetOrdersByUserIdAsync(user.Id);

            foreach (var order in orders)
            {
                // Kiểm tra đơn hàng đã xác nhận, quá 7 ngày so với ngày giao dự kiến, và chưa được cấp voucher
                if (order.Status == OrderStatus.Confirmed &&
                    order.EstimatedDeliveryDate.HasValue &&
                    DateTime.Now > order.EstimatedDeliveryDate.Value.AddDays(7) &&
                    !order.IsLateVoucherIssued)
                {
                    // Tạo một voucher mới
                    var newVoucher = new Voucher
                    {
                        Code = $"LATEDELIVERY-{order.Id}-{user.Id.Substring(0, 4)}".ToUpper(),
                        Type = VoucherType.FixedAmount,
                        DiscountValue = 50000, // Giảm 50k
                        Quantity = 1,
                        ExpiryDate = DateTime.Now.AddMonths(3)
                    };
                    await _voucherRepository.AddAsync(newVoucher);

                    // Đánh dấu đã cấp voucher cho đơn hàng này
                    order.IsLateVoucherIssued = true;
                    await _orderRepository.UpdateOrderAsync(order);

                    TempData["VoucherMessage"] = $"Đơn hàng #{order.Id} của bạn đã bị giao trễ. Chúng tôi xin tặng bạn một voucher giảm giá: {newVoucher.Code}";
                }
            }

            return View(orders);
        }

        // Action Details và các action khác của OrderController sẽ được thêm ở đây sau
    }
}