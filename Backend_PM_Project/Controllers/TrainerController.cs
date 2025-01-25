using Backend_PM_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Backend_PM_Project.Controllers
{
    [Route("TrainerTable")]
    [ApiController]
    public class TrainerController : ControllerBase
    {
        private readonly PmProjectDatabaseContext _Context;
        public TrainerController(PmProjectDatabaseContext context)
        {
            _Context = context;

        }
        [HttpPost]
        public async Task<ActionResult> RegisterNewTrainer(RegisterNewTrainer newtrainer)
        {
            var newTrainer = new TrainerTable
            {
                TrainerId=Guid.NewGuid(),
                TrainerName=newtrainer.userName,
                TrainerEmail=newtrainer.userEmail,
                TrainerPassword=newtrainer.userPassword,
                Verified = false,
            };
            if (newTrainer != null)
            {
                _Context.TrainerTables.Add(newTrainer);
                await _Context.SaveChangesAsync();
                return StatusCode(201, new { message = "Sikeres Regisztráció!" });
            }
            return BadRequest(new { message = "Sikertelen Regisztráció!" });
        
        }
        [HttpGet]
        public async Task<ActionResult> GetAllTrainer()
        {
            var trainers = await _Context.TrainerTables.ToListAsync();
            if (trainers != null)
            {
                return Ok(trainers);
            }
            return BadRequest();
        }
    }
}
