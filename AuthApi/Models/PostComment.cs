using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AuthApi.Models;

public partial class PostComment
{
    [Key]
    public int CommentId { get; set; }

    public string CommenterName { get; set; } = null!;

    public string CommentContent { get; set; } = null!;

    public int PostId { get; set; }

    public virtual PostTable Post { get; set; } = null!;
}
