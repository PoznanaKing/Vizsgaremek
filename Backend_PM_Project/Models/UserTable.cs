using System;
using System.Collections.Generic;

namespace Backend_PM_Project.Models;

public partial class UserTable
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Userpassword { get; set; } = null!;

    public string Email { get; set; } = null!;

    public bool? Verified { get; set; }

    public virtual ICollection<PostTable> PostTables { get; set; } = new List<PostTable>();

    public virtual ICollection<TrainerUserMessageConnector> TrainerUserMessageConnectors { get; set; } = new List<TrainerUserMessageConnector>();
}
