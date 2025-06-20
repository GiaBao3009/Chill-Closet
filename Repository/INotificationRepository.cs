using Chill_Closet.Models;
namespace Chill_Closet.Repository
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(string userId);
        Task MarkAsReadAsync(IEnumerable<Notification> notifications);
    }
}