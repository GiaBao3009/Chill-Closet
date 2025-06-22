using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chill_Closet.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string Message { get; set; } // Nội dung thông báo

        public bool IsRead { get; set; } = false; // Trạng thái đã đọc hay chưa

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string? Url { get; set; } // Đường link khi nhấn vào thông báo

        // Khóa ngoại đến người dùng sẽ nhận thông báo
        [Required]
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }
    }
}