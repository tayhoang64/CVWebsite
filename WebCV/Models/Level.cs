using System;
using System.Collections.Generic;

namespace WebCV.Models;

public partial class Level
{
    public int LevelId { get; set; }

    public string? LevelName { get; set; }

    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
