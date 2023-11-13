using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class User
{
    public decimal Userid { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? Email { get; set; }

    public DateTime? Registrationdate { get; set; }

    public decimal? Roleid { get; set; }

    public string? Address { get; set; }

    public string? Phonenumber { get; set; }

    public decimal? Currencyid { get; set; }

    public virtual Currency? Currency { get; set; }

    public virtual ICollection<Emailnotification> Emailnotifications { get; set; } = new List<Emailnotification>();

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Role? Role { get; set; }

    public virtual ICollection<Shoppingcart> Shoppingcarts { get; set; } = new List<Shoppingcart>();

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();

    public virtual ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
