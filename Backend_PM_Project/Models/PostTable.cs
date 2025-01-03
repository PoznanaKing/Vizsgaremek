using System;
using System.Collections.Generic;

namespace Backend_PM_Project.Models;

public partial class PostTable
{
    public Guid PostId { get; set; }

    public string PostTitle { get; set; } = null!;

    public string? PostDescription { get; set; }

    public byte[] PostImage { get; set; }

    public Guid Userid { get; set; }

    public virtual UserTable User { get; set; } = null!;
}
