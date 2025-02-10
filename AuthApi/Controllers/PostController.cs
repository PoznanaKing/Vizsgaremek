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
    [Route("Posttable")]
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

        
        [HttpPost("UploadPost")]
        public async Task<ActionResult> UploadPost([FromForm] UploadPostDTO postDTO)
        {
            byte[] postImageBytes = null;
            if (postDTO.post_image != null && postDTO.post_image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await postDTO.post_image.CopyToAsync(memoryStream);
                    postImageBytes = memoryStream.ToArray();
                }
            }

            var result = new PostTable
            {
                PostTitle = postDTO.post_title,
                PostDescription = postDTO.post_description,
                PostImage = postImageBytes,
                UserId = postDTO.user_id,
            };
            await _context.AddAsync(result);
            await _context.SaveChangesAsync();

            if (result != null)
            {
                return StatusCode(201, new { result, message = "Sikeres hozzáadás." });
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
