using System;
using System.Collections.Generic;

namespace WebCV.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string? Content { get; set; }

    public DateTime? SendAt { get; set; }

    public int? Hide { get; set; }

    public string? Link { get; set; }

    public virtual User User { get; set; } = null!;
}
