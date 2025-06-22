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
        private readonly INotificationRepository _notificationRepository;
        private readonly IReturnRequestRepository _returnRequestRepository; 

        public OrderController(IOrderRepository orderRepository,
                               UserManager<ApplicationUser> userManager,
                               IVoucherRepository voucherRepository,
                               INotificationRepository notificationRepository,
                               IReturnRequestRepository returnRequestRepository) // <-- 2. THÊM VÀO CONSTRUCTOR
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _voucherRepository = voucherRepository;
            _notificationRepository = notificationRepository;
            _returnRequestRepository = returnRequestRepository;

        }

        // MỘT ACTION INDEX DUY NHẤT
        // Mở file Controllers/OrderController.cs và thay thế toàn bộ action Index này

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var orders = await _orderRepository.GetOrdersByUserIdAsync(user.Id);

            foreach (var order in orders)
            {
                if ((order.Status == OrderStatus.Confirmed || order.Status == OrderStatus.Shipping) &&
                    order.EstimatedDeliveryDate.HasValue &&
                    DateTime.Now > order.EstimatedDeliveryDate.Value &&
                    !order.IsLateVoucherIssued)
                {
                    var newVoucher = new Voucher
                    {
                        Code = $"LATE-{order.Id}-{user.Id.Substring(0, 4)}".ToUpper(),
                        Type = VoucherType.FixedAmount,
                        DiscountValue = 50000, // Giảm 50k
                        Quantity = 1,
                        ExpiryDate = DateTime.Now.AddMonths(1),
                        ApplicationUserId = user.Id
                    };
                    
                    await _voucherRepository.AddAsync(newVoucher);

                    // TẠO THÔNG BÁO MỚI
                    var notification = new Notification
                    {
                        ApplicationUserId = user.Id,
                        Message = $"Bạn có voucher mới '{newVoucher.Code}' cho đơn hàng #{order.Id} bị giao trễ.",
                        Url = Url.Action("Index", "Voucher") // Sau này sẽ tạo trang quản lý voucher cho user
                    };
                    await _notificationRepository.AddAsync(notification);

                    order.IsLateVoucherIssued = true;
                    await _orderRepository.UpdateOrderAsync(order);
                }
            }
            return View(orders);
        }
        // GET: /Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // --- KIỂM TRA BẢO MẬT ---
            // Đảm bảo rằng người dùng hiện tại chỉ có thể xem đơn hàng của chính họ.
            var user = await _userManager.GetUserAsync(User);
            if (order.ApplicationUserId != user.Id)
            {
                return Forbid(); // Hoặc NotFound() để không tiết lộ sự tồn tại của đơn hàng
            }

            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelivery(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra bảo mật: chỉ chủ đơn hàng mới có quyền xác nhận
            var user = await _userManager.GetUserAsync(User);
            if (order.ApplicationUserId != user.Id)
            {
                return Forbid();
            }

            // Chỉ cập nhật trạng thái nếu đơn hàng đang ở trạng thái "Shipping"
            if (order.Status == OrderStatus.Shipping)
            {
                order.Status = OrderStatus.Completed;
                await _orderRepository.UpdateOrderAsync(order);
                TempData["SuccessMessage"] = "Cảm ơn bạn đã xác nhận đã nhận được hàng!";
            }

            return RedirectToAction("Details", new { id = id });
        }

        // GET: /Order/RequestReturn/5
        public async Task<IActionResult> RequestReturn(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null /*... các kiểm tra bảo mật khác ...*/)
            {
                return NotFound();
            }

            var viewModel = new ReturnRequestViewModel
            {
                OrderId = id
            };
            return View(viewModel);
        }

        // POST: /Order/RequestReturn/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestReturn(ReturnRequestViewModel viewModel)
        {
            // Gán lại OrderId vào ModelState để validation hoạt động đúng
            ModelState.SetModelValue("OrderId", new Microsoft.AspNetCore.Mvc.ModelBinding.ValueProviderResult(viewModel.OrderId.ToString()));

            if (ModelState.IsValid)
            {
                // Tạo đối tượng ReturnRequest thực sự để lưu vào DB
                var returnRequest = new ReturnRequest
                {
                    OrderId = viewModel.OrderId,
                    Reason = viewModel.Reason,
                    ContactPhone = viewModel.ContactPhone,
                    RefundMethod = viewModel.RefundMethod,
                    BankName = viewModel.BankName,
                    BankAccountNumber = viewModel.BankAccountNumber,
                    BankAccountName = viewModel.BankAccountName,
                    MomoPhoneNumber = viewModel.MomoPhoneNumber,
                    Status = ReturnStatus.Pending,
                    RequestDate = DateTime.Now
                };

                if (viewModel.ImageFiles != null && viewModel.ImageFiles.Any())
                {
                    foreach (var imageFile in viewModel.ImageFiles)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/returns", fileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        returnRequest.Images.Add(new ReturnRequestImage { Url = "/img/returns/" + fileName });
                    }
                }

                await _returnRequestRepository.AddAsync(returnRequest);

                var order = await _orderRepository.GetOrderByIdAsync(returnRequest.OrderId);
                if (order != null)
                {
                    order.Status = OrderStatus.ReturnRequested;
                    await _orderRepository.UpdateOrderAsync(order);
                }

                return RedirectToAction("RequestReturnCompleted");
            }

            // Nếu lỗi, trả về form với dữ liệu đã nhập
            return View(viewModel);
        }
        public IActionResult RequestReturnCompleted()
        {
            return View();
        }
        // Action Details và các action khác của OrderController sẽ được thêm ở đây sau
    }
}