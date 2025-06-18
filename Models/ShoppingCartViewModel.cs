namespace Chill_Closet.Models
{
    public class ShoppingCartViewModel
    {
        public ShoppingCart ShoppingCart { get; set; }
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal GrandTotal { get; set; }

        public string? AppliedVoucherCode { get; set; }
    }
}