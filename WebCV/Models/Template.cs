using System;
using System.Collections.Generic;

namespace WebCV.Models;

public partial class Template
{
    public int TemplateId { get; set; }

    public string? Title { get; set; }

    public string? File { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? Hide { get; set; }

    public string? UploadedBy { get; set; }

    public DateTime? LastUpdatedAt { get; set; }

    public string? Link { get; set; }

    public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();
}
