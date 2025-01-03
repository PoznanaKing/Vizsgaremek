using Backend_PM_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend_PM_Project.Controllers
{
    [Route("PostTable")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PmProjectDatabaseContext _Context;
        public PostController(PmProjectDatabaseContext context)
        {
            _Context = context;

        }
        [HttpPut("NewPost")]
        public async Task<ActionResult> NewPost(NewPostDTO newPost)
        {
            var post = new PostTable
            {
                PostId = Guid.NewGuid(),
                PostTitle=newPost.postTitle,
                PostDescription=newPost.postDesc,
                PostImage=newPost.postImg,
                Userid=newPost.userId,
            };
            if (post!=null)
            {
                await _Context.PostTables.AddAsync(post);
                _Context.SaveChanges();
                return StatusCode(201, new { message = "Sikeres feltöltés." });
            }
            return BadRequest(new {message = "Sikertelen feltöltés."});
        }
    }
}
