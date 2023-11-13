namespace ProjectMVC.Models
{
    internal class CartItemViewModel
    {
        public decimal CartId { get; set; }
        public decimal ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        
    }
}