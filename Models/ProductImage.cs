using System.ComponentModel.DataAnnotations;

namespace Chill_Closet.Models
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Url { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}