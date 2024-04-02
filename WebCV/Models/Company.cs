using System;
using System.Collections.Generic;

namespace WebCV.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string? CopmpanyName { get; set; }

    public string? Address { get; set; }

    public string? Country { get; set; }

    public string? Website { get; set; }

    public string? Hotline { get; set; }

    public string? Information { get; set; }

    public string? Link { get; set; }

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
