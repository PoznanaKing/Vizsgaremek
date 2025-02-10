namespace AuthApi.Models.Dtos
{
    public record UploadComment(string CommenterName,string CommentContent, int PostId);
    public record DeleteCommentDTO(int CommentId);
    public record UpdateCommentDTO(int CommentId,string CommentContent);
    
}
