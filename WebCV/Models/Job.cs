using System;
using System.Collections.Generic;

namespace WebCV.Models;

public partial class Job
{
    public int JobId { get; set; }

    public int CategoryId { get; set; }

    public int LevelId { get; set; }

    public int CompanyId { get; set; }

    
    public string? JobName { get; set; }

    public string? Jd { get; set; }

    public int? ExperienceYear { get; set; }

    public DateTime? UpdateDay { get; set; }

    public DateTime? EndDay { get; set; }

    public int? RecruitmentCount { get; set; }

    public decimal? Salary { get; set; }

    public int? Status { get; set; }

    public string? Link { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual Level Level { get; set; } = null!;

    public virtual ICollection<Recruitment> Recruitments { get; set; } = new List<Recruitment>();

    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
}
