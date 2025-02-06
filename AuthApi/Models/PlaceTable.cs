using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models;

public partial class PlaceTable
{
    [Key]
    public Guid PlaceId { get; set; }

    public string PlaceName { get; set; } = null!;

    public int PostalCode { get; set; }

    public string TownName { get; set; } = null!;

    public string StreetName { get; set; } = null!;

    public int StoryLevel { get; set; }

    public string Description { get; set; } = null!;

    public double Rating { get; set; }
}
