using AuthApi.Models;
using AuthApi.Models.Dtos;

namespace AuthApi.Services.IService
{
    public interface IPostComment
    {
        Task<PostComment> UploadComment(UploadComment uploadComment);
        Task<PostComment> DeleteComment(DeleteCommentDTO deleteCommentDTO);
        Task<PostComment> UpdateComment(UpdateCommentDTO updateCommentDTO);
    }
}
