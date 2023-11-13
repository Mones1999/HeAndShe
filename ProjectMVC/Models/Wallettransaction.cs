using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Wallettransaction
{
    public decimal Transactionid { get; set; }

    public decimal? Walletid { get; set; }

    public string? Transactiontype { get; set; }

    public decimal? Amount { get; set; }

    public DateTime? Transactiondate { get; set; }

    public string? Description { get; set; }

    public virtual Wallet? Wallet { get; set; }
}
