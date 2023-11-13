using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Currency
{
    public decimal Currencyid { get; set; }

    public string? Currencycode { get; set; }

    public string? Currencyname { get; set; }

    public decimal? Exchangetobase { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
