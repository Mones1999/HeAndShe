namespace ProjectMVC.Models
{
    public class ProductReportViewModel
    {
        public decimal ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string CategoryName { get; set; }
        public decimal? Price { get; set; }
        public decimal? StockQuantity { get; set; }
        public string Status { get; set; }
        public decimal? Rating { get; set; }
        public int NumberOfSales { get; set; }
        public decimal TotalSalesRevenue { get; set; }
        public int NumberOfReviews { get; set; }
    }
}
