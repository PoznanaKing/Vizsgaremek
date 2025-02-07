using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services;
using AuthApi.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static AuthApi.Models.Dtos.postDTOs;

namespace AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPost post;

        public PostController(AppDbContext context, IPost post)
        {
            _context = context;
            this.post = post;
        }

        [Authorize(Roles = "User,Trainer,PlaceOwner")]
        [HttpPost("UploadPost")]
        public async Task<ActionResult> UploadPost(UploadPostDTO uploadPostDTO)
        {
            var newPost = post.uploadPost(uploadPostDTO);
            
            if (newPost != null)
            {
                PostTable finalUpload = new PostTable();
                finalUpload.PostTitle = uploadPostDTO.post_title;
                finalUpload.PostDescription = uploadPostDTO.post_description;
                finalUpload.UserId=uploadPostDTO.user_id;
                finalUpload.PostImage = uploadPostDTO.post_image;
                await _context.posts.AddAsync(finalUpload);
                await _context.SaveChangesAsync();
                return StatusCode(201, new { result = newPost, message = "Sikeres hozzáadás." });
            }
            return BadRequest(new { result = "", message = "Sikertelen hozzáadás." });
        }
        [Authorize(Roles = "User,Trainer,PlaceOwner")]
        [HttpGet("ById")]
        public async Task<ActionResult> GetPostById(GetPostWithIdDTO getPostWithIdDTO)
        {
            var poster = post.getPostWithId(getPostWithIdDTO);
            if (poster != null)
            {
                return Ok(new {result = poster});
            }
            return NotFound(new {result = poster});
        }
        [HttpDelete("DeletePost")]
        public async Task<ActionResult> DeletePostById(DeletePostDTO postDTO)
        {
            var deletingPost = await post.deletePost(postDTO);
            if (deletingPost != null)
            {
                _context.posts.Remove(deletingPost);
                await _context.SaveChangesAsync();
                return StatusCode(200, new { message = "Sikeres törlés." });
            }
            return NotFound(new {message="Sikertelen törlés."});

        }
    }
}
