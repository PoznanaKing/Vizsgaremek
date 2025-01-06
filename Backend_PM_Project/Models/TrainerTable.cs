using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Backend_PM_Project.Models;

public partial class TrainerTable
{
    public Guid TrainerId { get; set; }

    public string TrainerName { get; set; } = null!;

    public string TrainerEmail { get; set; } = null!;

    public string TrainerPassword { get; set; } = null!;
    
    public virtual ICollection<PlaceTable> PlaceTables { get; set; } = new List<PlaceTable>();
    
    public virtual ICollection<TrainerUserMessageConnector> TrainerUserMessageConnectors { get; set; } = new List<TrainerUserMessageConnector>();
}
