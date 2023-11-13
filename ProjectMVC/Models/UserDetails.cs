namespace ProjectMVC.Models
{
    public class UserDetails
    {
        public User User { get; set; }
        public List<Review> Review { get; set; }
        public List<Testimonial> Testimonial { get; set; }
        public Wallet Wallet { get; set; }
        public List<Order> Order { get; set; }

    }
}
