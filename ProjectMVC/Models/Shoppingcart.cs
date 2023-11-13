using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Shoppingcart
{
    public decimal Cartid { get; set; }

    public decimal? Userid { get; set; }

    public decimal? Productid { get; set; }

    public decimal? Quantity { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
