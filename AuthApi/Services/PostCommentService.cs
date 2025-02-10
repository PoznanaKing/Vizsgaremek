using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Services
{
    public class PostCommentService : IPostComment
    {
        private readonly AppDbContext _appDbContext;

        public PostCommentService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<PostComment> DeleteComment(DeleteCommentDTO deleteCommentDTO)
        {
            var deletingComment = await _appDbContext.comments.FirstOrDefaultAsync(x => x.CommentId == deleteCommentDTO.CommentId);
            if (deletingComment != null)
            {
                return deletingComment;
            }
            return null;
        }

        public async Task<PostComment> UpdateComment(UpdateCommentDTO updateCommentDTO)
        {
            var updatingComment = await _appDbContext.comments.FirstOrDefaultAsync(x=>x.CommentId == updateCommentDTO.CommentId);
            if (updatingComment!=null)
            {
                return updatingComment;
            }
            return null;
        }

        public async Task<PostComment> UploadComment(UploadComment uploadComment)
        {
            var newComment = new PostComment
            {
                CommentContent=uploadComment.CommentContent,
                CommenterName=uploadComment.CommenterName,
                PostId=uploadComment.PostId,
            };

            if (newComment != null)
            {
                return newComment;
            }
            return null;
        }
    }
}
