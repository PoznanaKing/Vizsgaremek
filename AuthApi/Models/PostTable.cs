using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthApi.Models;

public partial class PostTable
{
    [Key]
    public int PostId { get; set; }

    public string PostTitle { get; set; } = null!;

    public byte[]? PostImage { get; set; }

    public string PostDescription { get; set; } = null!;

    public string? UserId { get; set; }
    [JsonIgnore]
    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    public virtual Aspnetuser? User { get; set; }
}
