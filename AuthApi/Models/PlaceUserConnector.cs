using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models;

public partial class PlaceUserConnector
{
    
    public string UserId { get; set; } = null!;

    public Guid PlaceId { get; set; }

    public virtual PlaceTable Place { get; set; } = null!;

    public virtual Aspnetuser User { get; set; } = null!;
}
