using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Review
{
    public decimal Reviewid { get; set; }

    public decimal? Productid { get; set; }

    public decimal? Userid { get; set; }

    public decimal? Rating { get; set; }

    public string? Reviewtext { get; set; }

    public DateTime? Reviewdate { get; set; }

    public string? Username { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }
}
