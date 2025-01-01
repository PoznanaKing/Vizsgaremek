using System;
using System.Collections.Generic;

namespace Backend_PM_Project.Models;

public partial class PlaceTable
{
    public Guid Placeid { get; set; }

    public string PlaceName { get; set; } = null!;

    public int PostalCode { get; set; }

    public string TownName { get; set; } = null!;

    public string StreetName { get; set; } = null!;

    public int? StoryLevle { get; set; }

    public string? Description { get; set; }

    public double? Rating { get; set; }

    public Guid? TrainerId { get; set; }

    public Guid OwnerId { get; set; }

    public virtual PlaceOwnerTable Owner { get; set; } = null!;

    public virtual TrainerTable? Trainer { get; set; }
}
