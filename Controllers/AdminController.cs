using Chill_Closet.Enums;
using Chill_Closet.Models;
using Chill_Closet.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Chill_Closet.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class AdminController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IVoucherRepository _voucherRepository;
        private readonly IReturnRequestRepository _returnRequestRepository; // Sửa lại tên biến cho nhất quán
        private readonly INotificationRepository _notificationRepository;

        public AdminController(IProductRepository productRepository,
                               ICategoryRepository categoryRepository,
                               IOrderRepository orderRepository,
                               IVoucherRepository voucherRepository,
                               IReturnRequestRepository returnRequestRepository,
                               INotificationRepository notificationRepository) // Sửa lại tên tham số
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _orderRepository = orderRepository;
            _voucherRepository = voucherRepository;
            _returnRequestRepository = returnRequestRepository; // Sửa lại tên biến gán
            _notificationRepository = notificationRepository;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Products()
        {
            var products = await _productRepository.GetAllAsync();
            return View(products);
        }

        // GET: Hiển thị form thêm sản phẩm
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
            return View();
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Xử lý dữ liệu từ form thêm sản phẩm (phiên bản ĐÚNG)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, List<IFormFile> imageFiles)
        {
            if (ModelState.IsValid)
            {
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    // Quy ước: ảnh đầu tiên sẽ là ảnh chính (MainImageUrl)
                    var firstImage = imageFiles.First();
                    var firstImageFileName = Path.GetFileName(firstImage.FileName);
                    var firstImageFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/product", firstImageFileName);

                    using (var fileStream = new FileStream(firstImageFilePath, FileMode.Create))
                    {
                        await firstImage.CopyToAsync(fileStream);
                    }
                    product.MainImageUrl = "/img/product/" + firstImageFileName;

                    // Xử lý các ảnh còn lại (nếu có)
                    foreach (var imageFile in imageFiles.Skip(1))
                    {
                        var additionalFileName = Path.GetFileName(imageFile.FileName);
                        var additionalFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/product", additionalFileName);

                        using (var fileStream = new FileStream(additionalFilePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(fileStream);
                        }
                        // Thêm ảnh phụ vào danh sách
                        product.Images.Add(new ProductImage { Url = "/img/product/" + additionalFileName });
                    }
                }

                await _productRepository.AddAsync(product);
                return RedirectToAction(nameof(Products));
            }

            // Nếu model không hợp lệ, trả lại view với danh sách danh mục
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
            return View(product);
        }
        // ===============================================
        // ACTION MỚI CHO CHỨC NĂNG SỬA SẢN PHẨM
        // ===============================================

        // GET: Hiển thị form để sửa sản phẩm
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            // Gửi danh sách danh mục sang cho View
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }

        // ===============================================
        // CÁC ACTION MỚI CHO CHỨC NĂNG XÓA SẢN PHẨM
        // ===============================================

        // GET: Hiển thị trang xác nhận xóa
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Thực hiện xóa sản phẩm và chuyển hướng
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Products));
        }

        // POST: Xử lý dữ liệu từ form sửa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? mainImageFile, List<IFormFile> imageFiles)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy thông tin sản phẩm hiện tại từ DB để không làm mất các ảnh cũ
                    var existingProduct = await _productRepository.GetByIdAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật các thuộc tính cơ bản
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.Description = product.Description;
                    existingProduct.CategoryId = product.CategoryId;

                    // Xử lý upload ảnh chính mới (nếu có)
                    if (mainImageFile != null)
                    {
                        var fileName = Path.GetFileName(mainImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/product", fileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await mainImageFile.CopyToAsync(fileStream);
                        }
                        existingProduct.MainImageUrl = "/img/product/" + fileName;
                    }

                    // Xử lý upload các ảnh phụ mới (nếu có)
                    if (imageFiles != null && imageFiles.Count > 0)
                    {
                        foreach (var imageFile in imageFiles)
                        {
                            var additionalFileName = Path.GetFileName(imageFile.FileName);
                            var additionalFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/product", additionalFileName);
                            using (var fileStream = new FileStream(additionalFilePath, FileMode.Create))
                            {
                                await imageFile.CopyToAsync(fileStream);
                            }
                            existingProduct.Images.Add(new ProductImage { Url = "/img/product/" + additionalFileName, ProductId = existingProduct.Id });
                        }
                    }

                    await _productRepository.UpdateAsync(existingProduct);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (await _productRepository.GetByIdAsync(product.Id) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Products));
            }

            // Nếu model không hợp lệ, trả lại view
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name", product.CategoryId);
            return View(product);
        }
        // ===============================================
        // CÁC ACTION CHO CHỨC NĂNG QUẢN LÝ DANH MỤC
        // ===============================================

        // GET: Hiển thị danh sách danh mục
        public async Task<IActionResult> Categories()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return View(categories);
        }

        // GET: Hiển thị form thêm danh mục mới
        public IActionResult CreateCategory()
        {
            return View();
        }

        // POST: Xử lý dữ liệu từ form thêm danh mục mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                // Thêm [Required] vào thuộc tính Name trong Category.cs để validation hoạt động
                await _categoryRepository.AddAsync(category);
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        // GET: Hiển thị form sửa danh mục
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Xử lý sửa danh mục
        [HttpPost]
        public async Task<IActionResult> EditCategory(int id, Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                await _categoryRepository.UpdateAsync(category);
                return RedirectToAction(nameof(Categories));
            }
            return View(category);
        }

        // GET: Hiển thị trang xác nhận xóa danh mục
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Xử lý xóa danh mục
        [HttpPost, ActionName("DeleteCategory")]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            await _categoryRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Categories));
        }
        // GET: Admin/Orders
        public async Task<IActionResult> Orders()
        {
            var orders = await _orderRepository.GetAllOrdersAsync();
            return View(orders);
        }
        // GET: Admin/OrderDetails/5
        public async Task<IActionResult> OrderDetails(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Admin/UpdateOrderStatus
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus newStatus)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            // Logic cũ: cho phép chuyển từ Pending sang Confirmed hoặc Cancelled
            if (order.Status == OrderStatus.Pending && (newStatus == OrderStatus.Confirmed || newStatus == OrderStatus.Cancelled))
            {
                order.Status = newStatus;
                if (newStatus == OrderStatus.Confirmed)
                {
                    order.EstimatedDeliveryDate = DateTime.Now.AddDays(7);
                }
            }
            // LOGIC MỚI: Cho phép chuyển từ Confirmed sang Shipping
            else if (order.Status == OrderStatus.Confirmed && newStatus == OrderStatus.Shipping)
            {
                order.Status = newStatus;
            }
            // Sau này có thể thêm các logic khác, ví dụ: từ Shipping -> Completed

            await _orderRepository.UpdateOrderAsync(order);

            TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
            return RedirectToAction("OrderDetails", new { id = orderId });
        }
        public async Task<IActionResult> Vouchers()
        {
            var vouchers = await _voucherRepository.GetAllAsync();
            return View(vouchers);
        }

        public IActionResult CreateVoucher() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVoucher(Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                await _voucherRepository.AddAsync(voucher);
                return RedirectToAction(nameof(Vouchers));
            }
            return View(voucher);
        }

        public async Task<IActionResult> EditVoucher(int id)
        {
            var voucher = await _voucherRepository.GetByIdAsync(id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVoucher(int id, Voucher voucher)
        {
            if (id != voucher.Id) return NotFound();
            if (ModelState.IsValid)
            {
                await _voucherRepository.UpdateAsync(voucher);
                return RedirectToAction(nameof(Vouchers));
            }
            return View(voucher);
        }

        public async Task<IActionResult> DeleteVoucher(int id)
        {
            var voucher = await _voucherRepository.GetByIdAsync(id);
            if (voucher == null) return NotFound();
            return View(voucher);
        }

        [HttpPost, ActionName("DeleteVoucher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVoucherConfirmed(int id)
        {
            await _voucherRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Vouchers));
        }
        // GET: Admin/ReturnRequests
        public async Task<IActionResult> ReturnRequests()
        {
            var requests = await _returnRequestRepository.GetAllAsync();
            return View(requests);
        }

        // GET: Admin/ReturnRequestDetails/5
        public async Task<IActionResult> ReturnRequestDetails(int id)
        {
            var request = await _returnRequestRepository.GetByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            return View(request);
        }

        // POST: Xử lý yêu cầu trả hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessReturnRequest(int id, ReturnStatus status)
        {
            var request = await _returnRequestRepository.GetByIdAsync(id);
            if (request == null) return NotFound();

            request.Status = status;
            await _returnRequestRepository.UpdateAsync(request);

            var notification = new Notification
            {
                ApplicationUserId = request.Order.ApplicationUserId,
                Message = $"Yêu cầu trả hàng cho đơn hàng #{request.OrderId} đã được cập nhật trạng thái thành '{status}'.",
                Url = Url.Action("Details", "Order", new { id = request.OrderId })
            };
            await _notificationRepository.AddAsync(notification);

            TempData["SuccessMessage"] = "Đã xử lý yêu cầu trả hàng.";
            return RedirectToAction("ReturnRequestDetails", new { id = id });
        }

    }
}