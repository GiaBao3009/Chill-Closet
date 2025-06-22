using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Chill_Closet.Models;
using Chill_Closet.Repository;

namespace Chill_Closet.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationRepository _notificationRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(INotificationRepository notificationRepo, UserManager<ApplicationUser> userManager)
        {
            _notificationRepo = notificationRepo;
            _userManager = userManager;
        }

        // API để lấy số lượng thông báo chưa đọc
        public async Task<IActionResult> GetUnreadCount()
        {
            var user = await _userManager.GetUserAsync(User);
            var notifications = await _notificationRepo.GetUnreadByUserIdAsync(user.Id);
            return Json(new { count = notifications.Count() });
        }

        // Lấy danh sách thông báo và hiển thị dưới dạng PartialView
        public async Task<IActionResult> GetNotificationsPartial()
        {
            var user = await _userManager.GetUserAsync(User);
            var notifications = await _notificationRepo.GetUnreadByUserIdAsync(user.Id);

            // Đánh dấu các thông báo này là đã đọc
            await _notificationRepo.MarkAsReadAsync(notifications);

            return PartialView("_NotificationList", notifications);
        }
    }
}