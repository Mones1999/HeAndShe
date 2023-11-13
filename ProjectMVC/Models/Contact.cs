using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Contact
{
    public decimal Contactid { get; set; }

    public string? Username { get; set; }

    public string? Email { get; set; }

    public string? Subject { get; set; }

    public string? Message { get; set; }
}
