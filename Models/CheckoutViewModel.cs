using System.ComponentModel.DataAnnotations;

namespace Chill_Closet.Models
{
    public class CheckoutViewModel
    {
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
    }
}