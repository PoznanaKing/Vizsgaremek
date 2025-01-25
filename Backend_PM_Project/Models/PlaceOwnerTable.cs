using System;
using System.Collections.Generic;

namespace Backend_PM_Project.Models;

public partial class PlaceOwnerTable
{
    public Guid OwnerId { get; set; }

    public string OwnerName { get; set; } = null!;

    public string OwnerEmail { get; set; } = null!;

    public string OwnerPassword { get; set; } = null!;

    public bool? Verified { get; set; }

    public virtual ICollection<PlaceTable> PlaceTables { get; set; } = new List<PlaceTable>();
}
