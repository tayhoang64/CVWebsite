using System;
using System.Collections.Generic;

namespace WebCV.Models;

public partial class Skill
{
    public int SkillId { get; set; }

    public string? SkillName { get; set; }

    public int JobId { get; set; }

    public virtual Job Job { get; set; } = null!;
}
