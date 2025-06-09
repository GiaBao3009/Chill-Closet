using System.ComponentModel.DataAnnotations.Schema;

namespace Chill_Closet.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }

        // === KHÓA NGOẠI ĐÚNG ĐẾN BẢNG IDENTITY ===
        // Id của người dùng đã đặt hàng
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
    }
}