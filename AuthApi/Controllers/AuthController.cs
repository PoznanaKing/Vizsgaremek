using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services.IService;
using emailApi.Services.IServices;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace AuthApi.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IAuth _auth;
        private readonly IEmail _email;
        private readonly AppDbContext _appDbContext;

        public AuthController(IMemoryCache memoryCache, IAuth auth, IEmail email, AppDbContext appDbContext)
        {
            _memoryCache = memoryCache;
            _auth = auth;
            _email = email;
            _appDbContext = appDbContext;
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginPost(LoginRequestDto loginRequestDto)
        {
            var log = await _auth.Login(loginRequestDto);

            if (log != null)
            {
                return Ok(log);
            }

            return BadRequest();
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterPost(RegisterRequestDto registerRequestDto)
        {
            var user = await _auth.Register(registerRequestDto);

            if (user != null)
            {
                Random rnd = new Random();
                int code = rnd.Next(100000, 999999); // 6 számjegyű kód generálása
                EmailDTO emailDTO = new EmailDTO(registerRequestDto.Email);
                _email.SendMail(emailDTO, code);

                // A kód tárolása a cache-ben 15 percre
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                _memoryCache.Set($"EmailVerificationCode_{registerRequestDto.Email}", code, cacheEntryOptions);

                return Ok(user);
            }

            return BadRequest();
        }

        [HttpPost("assignRole")]
        public async Task<ActionResult> AssignRole(AssignRoleRequestDto assignRoleRequestDto)
        {
            var user = await _auth.AssignRole(assignRoleRequestDto);

            if (user != null)
            {
                return StatusCode(201, user);
            }

            return BadRequest(user);
        }

        [HttpPut("EmailVerification")]
        public async Task<ActionResult> EmailVerify(int inputCode, string userId)
        {
            // Lekérjük a felhasználót a userId alapján
            var user = _appDbContext.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                return BadRequest(new { message = "A felhasználó nem található!" });
            }

            // A cache kulcsa az email cím, ahogy a regisztráció során mentettük
            if (_memoryCache.TryGetValue($"EmailVerificationCode_{user.Email}", out int cachedCode))
            {
                if (inputCode == cachedCode)
                {
                    user.EmailConfirmed = true;
                    _appDbContext.Update(user);
                    await _appDbContext.SaveChangesAsync();
                    return Ok(new { message = "Sikeres igazolás!" });
                }
            }

            return BadRequest(new { message = "Sikertelen igazolás, hibás a kód!" });
        }
        [HttpPost("SendEmailUserTrainer")]
        public async void SendMailFromTo(EmailFromToDTO emailFromToDTO)
        {
            _email.SendMailFromTo(emailFromToDTO);
        }
    }
}
