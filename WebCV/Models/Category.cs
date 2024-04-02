using System;
using System.Collections.Generic;

namespace WebCV.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
