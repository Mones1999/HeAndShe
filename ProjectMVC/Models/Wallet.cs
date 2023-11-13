using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Wallet
{
    public decimal Walletid { get; set; }

    public decimal? Userid { get; set; }

    public decimal? Balance { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User? User { get; set; }

    public virtual ICollection<Wallettransaction> Wallettransactions { get; set; } = new List<Wallettransaction>();
}
