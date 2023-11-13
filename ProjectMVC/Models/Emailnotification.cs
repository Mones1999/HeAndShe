using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Emailnotification
{
    public decimal Notificationid { get; set; }

    public decimal? Userid { get; set; }

    public string? Message { get; set; }

    public DateTime? Datesent { get; set; }

    public virtual User? User { get; set; }
}
