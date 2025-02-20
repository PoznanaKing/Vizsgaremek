using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services;
using AuthApi.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [Authorize(Roles = "Admin,PlaceOwner,User,Trainer")]
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



        [Authorize(Roles = "Admin,PlaceOwner,User,Trainer")]
        [HttpPost("ById")]
        public async Task<ActionResult> GetPostById([FromBody] GetPostWithIdDTO getPostWithIdDTO)
        {
            var poster = await post.getPostWithId(getPostWithIdDTO);
            if (poster != null)
            {
                return Ok(new { result = poster });
            }
            return NotFound(new { result = poster });
        }
        [Authorize(Roles = "Admin,PlaceOwner,User,Trainer")]
        [HttpPost("DeletePost")]  // Changed to POST because we're sending a body
        public async Task<ActionResult> DeletePostById([FromBody] DeletePostDTO postDTO)
        {
            var deletingPost = await post.deletePost(postDTO);
            if (deletingPost != null)
            {
                _context.posts.Remove(deletingPost);
                await _context.SaveChangesAsync();
                return StatusCode(200, new { message = "Sikeres törlés." });
            }
            return NotFound(new { message = "Sikertelen törlés." });
        }

        [Authorize(Roles = "Admin,PlaceOwner,User,Trainer")]
        [HttpPut("UpdatePost")]
        public async Task<ActionResult> UpdatePost([FromForm] UpdatePostDTO updatePostDTO)
        {
            // Ellenőrizzük, hogy van-e feltöltött kép
            byte[] postImageBytes = null;
            if (updatePostDTO.post_image != null && updatePostDTO.post_image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await updatePostDTO.post_image.CopyToAsync(memoryStream); // Aszinkron fájl másolás
                    postImageBytes = memoryStream.ToArray();
                }
            }

            // Frissítjük az adatbázisban lévő posztot
            var updatePost = await post.updatePost(updatePostDTO);

            if (updatePost != null)
            {
                // Ha van új kép, akkor frissítjük a poszt képét is
                updatePost.PostTitle = updatePostDTO.post_title;
                updatePost.PostDescription = updatePostDTO.post_description;
                updatePost.PostImage = postImageBytes;

                // Elmentjük az adatbázisba a frissítést
                _context.posts.Update(updatePost);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { result = updatePost, message = "Sikeres módosítás." });
            }
            return NotFound(new { result = updatePost, message = "Sikertelen módosítás." });
        }
        [Authorize(Roles = "Admin,PlaceOwner,User,Trainer")]
        [HttpGet("GetAllPostsWithComments")]
        public async Task<ActionResult> GetAllPosts()
        {
            var datas = await _context.posts.Include(x=>x.PostComments).ToListAsync();

            if (datas != null && datas.Any())
            {
                return Ok(datas);
            }

            return NotFound(new { message = "Nincs semmi az adatbázisban." });
        }
    }
}
