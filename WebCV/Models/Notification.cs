using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebCV.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public string? Content { get; set; }

    public DateTime? SendAt { get; set; }

    public int? Hide { get; set; }

    public string? Link { get; set; }

    [Newtonsoft.Json.JsonIgnore]
    public virtual User User { get; set; } = null!;
}
