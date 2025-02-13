using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        [HttpPut("EditPlaceData")]
        public async Task<ActionResult> EditPlaceData(EditPlaceDTO editPlaceDTO)
        {
            // Lekérdezzük a meglévő entitást az adatbázisból
            var existingPlace = await _context.places.FindAsync(editPlaceDTO.placeid);
            if (existingPlace == null)
            {
                return NotFound(new { message = "A hely nem található." });
            }

            // Frissítjük a meglévő entitás tulajdonságait
            existingPlace.PlaceName = editPlaceDTO.placename;
            existingPlace.Description = editPlaceDTO.description;
            existingPlace.PostalCode = editPlaceDTO.postalcode;
            existingPlace.Rating = editPlaceDTO.rating;
            existingPlace.StoryLevel = editPlaceDTO.storylevel;
            existingPlace.StreetName = editPlaceDTO.streetname;
            existingPlace.TownName = editPlaceDTO.townname;

            // Mentjük a változtatásokat
            _context.places.Update(existingPlace);
            await _context.SaveChangesAsync();

            return Ok(new { result = existingPlace, message = "Sikeres módosítás." });
        }
        [HttpDelete("DeletePost")]
        public async Task<ActionResult> DeletePlace(DeletePlaceDTO deletePlaceDTO)
        {

            if (deletePlaceDTO == null)
            {
                return BadRequest(new { message = "Érvénytelen adatok." });
            }
            
            var deletingPlace = await place.DeletePlace(deletePlaceDTO);
            if (deletingPlace != null)
            {
                _context.places.Remove(deletingPlace);
                await _context.SaveChangesAsync();
                return Ok(new { result = deletingPlace, message = "Sikeres törlés." });
            }

            return NotFound(new { message = "A hely nem található." });
        }
        [HttpGet("GetAllPlaces")]
        public async Task<ActionResult> GetAllPlaces()
        {
            var allPlaces = await _context.places.ToListAsync();
            if (allPlaces != null)
            {
                return Ok(allPlaces);
            }
            return NotFound(new {message="Nincs a rendszerben még edzőterem."});
        }
        [HttpPost("getplacebyid")]
        public async Task<ActionResult> GetPlaceById(GetPlaceById getPlaceById)
        {
            var placeById = await place.GetPlaceById(getPlaceById);
            if (placeById != null)
            {
                return Ok(new { result = placeById });
            }
            return NotFound(new { result = placeById, message = "Az adott id-val nem található edzőterem." });
        }
    }
}
