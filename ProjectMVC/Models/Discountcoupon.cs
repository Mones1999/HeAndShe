using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Discountcoupon
{
    public decimal Couponid { get; set; }

    public string? Couponcode { get; set; }

    public decimal? Discountpercentage { get; set; }

    public DateTime? Expirationdate { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
