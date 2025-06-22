using Microsoft.AspNetCore.Identity;

namespace Chill_Closet.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }

        public ICollection<Notification> Notifications { get; set; }
    }
}