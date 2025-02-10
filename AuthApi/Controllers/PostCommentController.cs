using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [Route("postcomment")]
    [ApiController]
    public class PostCommentController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPostComment comment;

        public PostCommentController(AppDbContext context, IPostComment Comment)
        {
            _context = context;
            this.comment = Comment;
        }

        [HttpPost("UploadComment")]
        public async Task<ActionResult> UploadComment(UploadComment uploadComment)
        {
            var newComment = await comment.UploadComment(uploadComment);
            if (newComment != null)
            {
                await _context.AddAsync(newComment);
                await _context.SaveChangesAsync();
                return StatusCode(201, new { result = newComment, message = "Sikeres feltöltés." });

            }
            return BadRequest(new { result = newComment, message = "Sikertelen feltöltés." });
        }
    }
}
