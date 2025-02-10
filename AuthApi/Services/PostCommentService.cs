using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services.IService;

namespace AuthApi.Services
{
    public class PostCommentService : IPostComment
    {
        private readonly AppDbContext _appDbContext;

        public PostCommentService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
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
