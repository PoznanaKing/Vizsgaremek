using AuthApi.Models.Dtos;
using AuthApi.Services.IService;
using emailApi.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth auth;
        private readonly IEmail email;

        public AuthController(IAuth auth, IEmail email)
        {
            this.auth = auth;
            this.email = email;
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginPost(LoginRequestDto loginRequestDto)
        {
            var log = await auth.Login(loginRequestDto);

            if (log != null)
            {
                return Ok(log);
            }

            return BadRequest();

        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterPost(RegisterRequestDto registerRequestDto)
        {
            var user = await auth.Register(registerRequestDto);

            if (user != null)
            {
                EmailDTO emailDTO = new EmailDTO(registerRequestDto.Email);
                email.SendMail(emailDTO);
                return Ok(user);
            }

            return BadRequest();

        }

        [HttpPost("assignRole")]
        public async Task<ActionResult> AssignRole(AssignRoleRequestDto assignRoleRequestDto)
        {
            var user = await auth.AssignRole(assignRoleRequestDto);

            if (user != null)
            {
                return StatusCode(201, user);
            }

            return BadRequest(user);
        }

    }
}
