namespace Chill_Closet.Models
{
    public class ShoppingCart
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();

        public void AddItem(CartItem item)
        {
            var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
            if (existingItem == null)
            {
                Items.Add(item);
            }
            else
            {
                existingItem.Quantity += item.Quantity;
            }
        }

        public void RemoveItem(int productId)
        {
            Items.RemoveAll(i => i.ProductId == productId);
        }

        public decimal GetSubtotal()
        {
            return Items.Sum(i => i.Total);
        }

        public decimal GetShippingFee()
        {
            return GetSubtotal() >= 500000 ? 0 : 30000;
        }

        // Phương thức tính tổng cuối cùng, có thể nhận voucher hoặc không
        public decimal GetGrandTotal(Voucher? voucher = null)
        {
            decimal subtotal = GetSubtotal();
            decimal shipping = GetShippingFee();
            decimal discount = 0;

            if (voucher != null)
            {
                discount = (voucher.Type == Enums.VoucherType.Percentage)
                    ? (subtotal * (voucher.DiscountValue / 100))
                    : voucher.DiscountValue;
            }

            return subtotal - discount + shipping;
        }
    }
}