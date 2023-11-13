using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Wishlist
{
    public decimal Wishlistitemid { get; set; }

    public decimal? Userid { get; set; }

    public decimal? Productid { get; set; }

    public DateTime? Dateadded { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
