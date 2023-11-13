using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Payment
{
    public decimal Paymentid { get; set; }

    public decimal? Orderid { get; set; }

    public decimal? Walletid { get; set; }

    public DateTime? Paymentdate { get; set; }

    public decimal? Amount { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Wallet? Wallet { get; set; }
}
