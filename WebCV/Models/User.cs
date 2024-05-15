using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebCV.Models;

public partial class User : IdentityUser<int>
{
    [Key]
    public int UserId { get; set; }

    public string? Avatar { get; set; }

    public string? FullName { get; set; }

    public int? Gender { get; set; }

    public string? Address { get; set; }

    public string? Country { get; set; }

    public string? Link { get; set; }

    public virtual ICollection<Cv> Cvs { get; set; } = new List<Cv>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Recruitment> Recruitments { get; set; } = new List<Recruitment>();
}
