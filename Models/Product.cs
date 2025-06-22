using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chill_Closet.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Giá là bắt buộc.")]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public string? MainImageUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        public int? CategoryId { get; set; } // SỬA Ở ĐÂY

        public Category? Category { get; set; }

        public List<ProductImage> Images { get; set; } = new List<ProductImage>();
    }
}