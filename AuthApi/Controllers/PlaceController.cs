using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthApi.Controllers
{
    [Route("PlaceTable")]
    [ApiController]
    public class PlaceController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly IPlace place;

        public PlaceController(AppDbContext context, IPlace place)
        {
            _context = context;
            this.place = place;
        }

        [HttpPost("UploadPlace")]
        public async Task<ActionResult> UploadPlace(UploadPlaceDTO uploadPlaceDTO)
        {
            var newPlace = await place.UploadPlace(uploadPlaceDTO); // Itt az `await` hiányzott
            if (newPlace != null)
            {
                await _context.places.AddAsync(newPlace); // Itt is az `await` hiányzott
                await _context.SaveChangesAsync(); // Mentés az adatbázisba
                return Ok(newPlace); // Visszatérés az új hely adataival
            }

            return BadRequest("Hiba történt a hely feltöltése során."); // Hibakezelés
        }
    }
}
