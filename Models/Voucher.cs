using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Chill_Closet.Enums;

namespace Chill_Closet.Models
{
    public class Voucher
    {
        public int Id { get; set; }

        [Required]
        public string Code { get; set; } // Mã voucher, ví dụ: "SALE50K"

        [Required]
        public VoucherType Type { get; set; } // Loại voucher: % hay số tiền

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal DiscountValue { get; set; } // Giá trị giảm (ví dụ: 10 cho 10%, hoặc 50000 cho 50k)

        [Required]
        public int Quantity { get; set; } // Số lượng voucher có sẵn

        [Required]
        public DateTime ExpiryDate { get; set; } // Ngày hết hạn
    }
}