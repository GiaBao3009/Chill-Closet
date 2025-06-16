using Chill_Closet.Models;
using Chill_Closet.Repository;
using Microsoft.AspNetCore.Authorization;
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

        // Constructor đúng, inject cả hai repository
        public AdminController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
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
    }
}