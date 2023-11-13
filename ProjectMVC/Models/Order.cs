using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Order
{
    public decimal Orderid { get; set; }

    public decimal? Userid { get; set; }

    public DateTime? Orderdate { get; set; }

    public decimal? Totalamount { get; set; }

    public string? Status { get; set; }

    public string? Shipmenttrackingnumber { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; set; } = new List<Orderdetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }
}
