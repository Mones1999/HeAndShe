using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Category
{
    public decimal Categoryid { get; set; }

    public string? Categoryname { get; set; }

    public decimal? Genderid { get; set; }

    public virtual Gender? Gender { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
