using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

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
        [Authorize(Roles = "Admin,PlaceOwner,User,Trainer")]
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
        [Authorize(Roles = "Admin,PlaceOwner,User,Trainer")]
        [HttpDelete("DeleteComment")]
        public async Task<ActionResult> DeleteCommentById(DeleteCommentDTO deleteCommentDTO)
        {
            var deletingComment = await comment.DeleteComment(deleteCommentDTO);
            
            if (deletingComment != null)
            {
                 _context.comments.Remove(deletingComment);
                await _context.SaveChangesAsync();
                return Ok(new {result=deletingComment, message="Sikeres törlés." });
            }
            return NotFound(new {result=deletingComment, message="Sikertelen törlés."});
        }
        [Authorize(Roles = "Admin,PlaceOwner,User,Trainer")]
        [HttpPut("UpdatePost")]
        public async Task<ActionResult> UpdateCommentById(UpdateCommentDTO updateCommentDTO)
        {
            var updatingComment= await comment.UpdateComment(updateCommentDTO);
            if(updatingComment != null)
            {
                updatingComment.CommentContent=updateCommentDTO.CommentContent;
                _context.comments.Update(updatingComment);
                await _context.SaveChangesAsync();
                return Ok(new {result=updatingComment,message="Sikeres módosítás."});
            }
            return NotFound(new { result = updatingComment, message = "Sikertelen módosítás." });
        }
    }
}
