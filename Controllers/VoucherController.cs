using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Chill_Closet.Models;
using Chill_Closet.Repository;

namespace Chill_Closet.Controllers
{
    [Authorize]
    public class VoucherController : Controller
    {
        private readonly IVoucherRepository _voucherRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public VoucherController(IVoucherRepository voucherRepository, UserManager<ApplicationUser> userManager)
        {
            _voucherRepository = voucherRepository;
            _userManager = userManager;
        }

        // GET: /Voucher
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var vouchers = await _voucherRepository.GetAvailableForUserAsync(user.Id);
            return View(vouchers);
        }
    }
}