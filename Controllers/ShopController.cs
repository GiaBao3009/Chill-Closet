using Chill_Closet.Models;
using Chill_Closet.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Chill_Closet.Controllers
{
    public class ShopController : Controller
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ShopController(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            var categories = await _categoryRepository.GetAllAsync();

            var shopViewModel = new ShopViewModel
            {
                Products = products,
                Categories = categories
            };

            return View(shopViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}