using Backend_PM_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_PM_Project.Controllers
{
    [Route("UserTable")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly PmProjectDatabaseContext _Context;
        public UserController(PmProjectDatabaseContext context) {
            _Context = context;

        }

        [HttpPost("RegisterNewUser")]
        public async Task<ActionResult> RegisterNewUser(RegisterNewUser newUser)
        {
            
            var newuser = new UserTable
            {
                Id = Guid.NewGuid(),
                Username = newUser.userName,
                Userpassword = newUser.userPassword,
                Email = newUser.userEmail,
            };
            if (newuser != null)
            {
                _Context.UserTables.Add(newuser);
                await _Context.SaveChangesAsync();
                return StatusCode(201, new { message = "Sikeres Regisztráció!" });
            }
            return BadRequest(new { message = "Sikertelen Regisztráció!" });
        }

        [HttpGet("GetExistingUser")]
        public async Task<ActionResult> GetExistingUser()
        {
            var users = await _Context.UserTables.ToListAsync();
            if (users != null)
            {
                return Ok(users);
            }
            return StatusCode(404, new { message = "Nincs felhasználó az adatbázisban!" });
        }
        [HttpPut("ResetUserPassword")]
        public async Task<ActionResult> ResetUserPassword(ResetPasswordDTO newPass)
        {
            var user = await _Context.UserTables.FirstOrDefaultAsync(x=>x.Id==newPass.userId);
            if (user !=null)
            {
                user.Userpassword = newPass.userNewPassword;
                _Context.UserTables.Update(user);
                _Context.SaveChanges();
                return StatusCode(200, new { message = "Sikeres jelszó változtatás!" });

            }
            return NotFound(new { message = "Sikertelen a jelszó váltás!" });
        }
        [HttpDelete("DeleteUserById")]
        public async Task<ActionResult> DeleteUserById(DeleteUserByIdDTO deleteUser)
        {
            var user = await _Context.UserTables.FirstOrDefaultAsync(x=>x.Id== deleteUser.userId);
            if (user!=null)
            {
                _Context.UserTables.Remove(user);
                _Context.SaveChanges();
                return StatusCode(200, new { message = "Sikeres törlés!" });
            }
            return NotFound(new { message = "Sikertelen törlés, mert a keresett azonosító alatt nincs ilyen felhasználó." });
        }

    }
}
