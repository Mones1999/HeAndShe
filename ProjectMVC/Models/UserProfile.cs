namespace ProjectMVC.Models
{
    public class UserProfile
    {
        public User User { get; set; }
        public Wallet Wallet { get; set; }
        public List<Wallettransaction> Wallettransaction { get; set; }
        public List<Order> Order { get; set; }
    }
}
