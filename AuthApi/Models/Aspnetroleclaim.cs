using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models;

public partial class Aspnetroleclaim
{
    [Key]
    public int Id { get; set; }

    public string RoleId { get; set; } = null!;

    public string? ClaimType { get; set; }

    public string? ClaimValue { get; set; }

    public virtual Aspnetrole Role { get; set; } = null!;
}
