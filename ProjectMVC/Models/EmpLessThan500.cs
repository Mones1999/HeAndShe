using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class EmpLessThan500
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public decimal Salary { get; set; }
}
