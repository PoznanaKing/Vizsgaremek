using System;
using System.Collections.Generic;

namespace Backend_PM_Project.Models;

public partial class TrainerUserMessageConnector
{
    public Guid ChatId { get; set; }

    public Guid UserId { get; set; }

    public Guid TrainerId { get; set; }

    public virtual ICollection<MessageContent> MessageContents { get; set; } = new List<MessageContent>();

    public virtual TrainerTable Trainer { get; set; } = null!;

    public virtual UserTable User { get; set; } = null!;
}
