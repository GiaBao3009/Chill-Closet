namespace Chill_Closet.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // Navigation property: Một danh mục có nhiều sản phẩm
        public List<Product> Products { get; set; }
    }
}