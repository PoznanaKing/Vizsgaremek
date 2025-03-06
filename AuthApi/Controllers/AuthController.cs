using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services.IService;
using emailApi.Services.IServices;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

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

                return Ok(new {user,code});
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
        [HttpGet("users")]
        public async Task<ActionResult> GetAllUsers()
        {
            var users = await _appDbContext.applicationUsers
                .Select(u => new
                {
                    userId = u.Id,
                    username = u.UserName,
                    email = u.Email,
                    roles = _appDbContext.UserRoles
                                .Where(ur => ur.UserId == u.Id)
                                .Join(_appDbContext.Roles,
                                      ur => ur.RoleId,
                                      r => r.Id,
                                      (ur, r) => r.Name)
                                .ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            var user = await _appDbContext.applicationUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound(new { message = "Felhasználó nem található!" });
            }
            _appDbContext.applicationUsers.Remove(user);
            await _appDbContext.SaveChangesAsync();
            return Ok(new { message = "Felhasználó törölve." });
        }
        [HttpGet("userById")]

        public async Task<ActionResult> GetUserById(string id)
        {
            var user = await _appDbContext.applicationUsers.FirstOrDefaultAsync(u => u.Id == id);
            if (user!=null)
            {
                UserByIdDTO userByIdDTO = new UserByIdDTO(user.UserName, user.Id, user.Email);
                
                return Ok(userByIdDTO);
            }
            return NotFound();
        }
        [HttpPost("sendMessage")]
        public async Task<ActionResult> SendMessage([FromBody] SendMessageRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sender = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.SenderId);
            var receiver = await _appDbContext.Users.FirstOrDefaultAsync(u => u.Id == request.ReceiverId);

            if (sender == null || receiver == null)
            {
                return BadRequest(new { message = "A küldő vagy fogadó nem található!" });
            }

            if (string.IsNullOrEmpty(receiver.Email))
            {
                return BadRequest(new { message = "A fogadónak nincs érvényes email címe!" });
            }

            _email.SendMessageEmail(receiver.Email, sender.UserName, request.Content);

            return Ok(new { message = "Üzenet sikeresen elküldve!" });
        }


    }
}
