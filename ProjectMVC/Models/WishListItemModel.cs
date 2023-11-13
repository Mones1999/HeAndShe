namespace ProjectMVC.Models
{
    public class WishListItemModel
    {
        public decimal Wishlistid { get; set; }
        public decimal ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        public string Status { get; set; }


        public DateTime? Dateadded { get; set; }
    }
}
