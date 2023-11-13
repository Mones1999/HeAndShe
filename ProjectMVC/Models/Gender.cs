using System;
using System.Collections.Generic;

namespace ProjectMVC.Models;

public partial class Gender
{
    public decimal Genderid { get; set; }

    public string? Gendername { get; set; }

    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
}
