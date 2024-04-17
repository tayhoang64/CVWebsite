using System;
using System.Collections.Generic;

namespace WebCV.Models;

public partial class Cv
{
    public int CvId { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public string? File { get; set; }

    public int UserId { get; set; }

    public int TemplateId { get; set; }

    public string? Image {  get; set; }

    public virtual Template Template { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
