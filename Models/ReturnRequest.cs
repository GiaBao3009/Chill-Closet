using Chill_Closet.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chill_Closet.Models
{
    public class ReturnRequest
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public Order Order { get; set; }

        [Required(ErrorMessage = "Vui lòng cho biết lý do trả hàng.")]
        public string Reason { get; set; }

        [Required(ErrorMessage = "Vui lòng cung cấp số điện thoại liên hệ.")]
        [Phone]
        public string ContactPhone { get; set; }

        public ReturnStatus Status { get; set; } = ReturnStatus.Pending;
        public DateTime RequestDate { get; set; } = DateTime.Now;
        [Required(ErrorMessage = "Vui lòng chọn phương thức hoàn tiền.")]
        public RefundMethod RefundMethod { get; set; }

        // Dành cho chuyển khoản ngân hàng
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankAccountName { get; set; }

        // Dành cho Momo
        public string? MomoPhoneNumber { get; set; }

        public List<ReturnRequestImage> Images { get; set; } = new List<ReturnRequestImage>();
    }
}