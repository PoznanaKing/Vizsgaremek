using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Backend_PM_Project.Models;

public partial class UserTable
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Userpassword { get; set; } = null!;

    public string Email { get; set; } = null!;
    
    public virtual ICollection<PostTable> PostTables { get; set; } = new List<PostTable>();
    [JsonIgnore]
    public virtual ICollection<TrainerUserMessageConnector> TrainerUserMessageConnectors { get; set; } = new List<TrainerUserMessageConnector>();
}
