using System;
using System.Collections.Generic;

namespace Backend_PM_Project.Models;

public partial class MessageContent
{
    public Guid Id { get; set; }

    public Guid MessageSenderId { get; set; }

    public string MessageContent1 { get; set; } = null!;

    public DateTime? MessageSentTime { get; set; }

    public Guid? ChatId { get; set; }

    public virtual TrainerUserMessageConnector? Chat { get; set; }
}
