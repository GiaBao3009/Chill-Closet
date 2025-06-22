using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Chill_Closet.Enums; // Cần thêm using này

namespace Chill_Closet.Models
{
    public class Order
    {
        public int Id { get; set; }

        // Các thuộc tính cho thông tin thanh toán của khách hàng
        [Required(ErrorMessage = "Họ và Tên là bắt buộc")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone]
        public string ShippingPhone { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress]
        public string Email { get; set; }

        public string? Notes { get; set; }

        // --- Các thuộc tính hệ thống ---
        public OrderStatus Status { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? EstimatedDeliveryDate { get; set; }
        public bool IsLateVoucherIssued { get; set; } = false;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        // --- Khóa ngoại đến người dùng ---
        public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }

        // --- Danh sách các sản phẩm trong đơn hàng ---
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}