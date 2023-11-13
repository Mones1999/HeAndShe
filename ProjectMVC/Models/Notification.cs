using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Notification
{
    public decimal Notificationid { get; set; }

    public decimal? Userid { get; set; }

    public string? Type { get; set; }

    public string? Status { get; set; }

    public DateTime? Datesent { get; set; }

    public virtual User? User { get; set; }
}
