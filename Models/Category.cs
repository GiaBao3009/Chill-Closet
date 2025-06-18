using System.ComponentModel.DataAnnotations; // Thêm using này

namespace Chill_Closet.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc.")]
        public string Name { get; set; }

        public List<Product>? Products { get; set; }
    }
}