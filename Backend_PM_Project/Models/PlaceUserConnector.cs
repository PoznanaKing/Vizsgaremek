using System;
using System.Collections.Generic;

namespace Backend_PM_Project.Models;

public partial class PlaceUserConnector
{
    public Guid? Userid { get; set; }

    public Guid? Placeid { get; set; }

    public virtual PlaceTable? Place { get; set; }

    public virtual UserTable? User { get; set; }
}
