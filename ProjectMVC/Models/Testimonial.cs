using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Testimonial
{
    public decimal Testimonialid { get; set; }

    public decimal? Userid { get; set; }

    public string? Username { get; set; }

    public string? Testimonialcontent { get; set; }

    public string? Status { get; set; }

    public virtual User? User { get; set; }
}
