namespace Project.Models
{
    public class CartItem
    {
        public int CartItemId {  get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public Cart cart { get; set; } = null!;
        public Product product { get; set; } = null!;
    }
}
