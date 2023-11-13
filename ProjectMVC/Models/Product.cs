using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectMVC.Models;

public partial class Product
{
    public decimal Productid { get; set; }

    public string? Productname { get; set; }

    public decimal? Categoryid { get; set; }

    public string? Description { get; set; }

    public decimal? Price { get; set; }

    public decimal? Stockquantity { get; set; }

    public string? Status { get; set; }

    public string? Imageurl { get; set; }

    public decimal? Rating { get; set; }

    public decimal? Numberofreviews { get; set; }

    public string? Productsize { get; set; }

    public decimal? Couponid { get; set; }

    [NotMapped]
    public IFormFile? ImageFile { get; set; }

    public virtual Category? Category { get; set; }

    public virtual Discountcoupon? Coupon { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Shoppingcart> Shoppingcarts { get; set; } = new List<Shoppingcart>();

    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
