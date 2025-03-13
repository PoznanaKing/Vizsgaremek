using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services.IService;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles ="Admin,PlaceOwner")]
        [HttpPost("UploadPlace")]
        public async Task<ActionResult> UploadPlace(UploadPlaceDTO uploadPlaceDTO)
        {
            if (uploadPlaceDTO == null)
            {
                return BadRequest("Érvénytelen adatok.");
            }

            var newPlace = await place.UploadPlace(uploadPlaceDTO);
            if (newPlace != null)
            {
                await _context.places.AddAsync(newPlace);
                await _context.SaveChangesAsync();
                return Ok(newPlace);
            }

            return BadRequest("Hiba történt a hely feltöltése során.");
        }
        [Authorize(Roles = "Admin,PlaceOwner")]
        [HttpPut("EditPlaceData/{placeId}")]
        public async Task<ActionResult> EditPlaceData(int placeId, EditPlaceDTO editPlaceDTO)
        {
            var existingPlace = await _context.places.FindAsync(placeId);
            if (existingPlace == null)
            {
                return NotFound(new { message = "A hely nem található." });
            }

            existingPlace.PlaceName = editPlaceDTO.placename;
            existingPlace.Description = editPlaceDTO.description;
            existingPlace.PostalCode = editPlaceDTO.postalcode;
            existingPlace.Rating = editPlaceDTO.rating;
            existingPlace.StoryLevel = editPlaceDTO.storylevel;
            existingPlace.StreetName = editPlaceDTO.streetname;
            existingPlace.TownName = editPlaceDTO.townname;

            _context.places.Update(existingPlace);
            await _context.SaveChangesAsync();

            return Ok(new { result = existingPlace, message = "Sikeres módosítás." });
        }

        [Authorize(Roles = "Admin,PlaceOwner")]
        [HttpDelete("DeletePost/{placeId}")]
        public async Task<ActionResult> DeletePlace(int placeId)
        {
            if (placeId <= 0)
            {
                return BadRequest(new { message = "Érvénytelen adatok." });
            }

            var deletePlaceDTO = new DeletePlaceDTO(placeId);
            var deletingPlace = await place.DeletePlace(deletePlaceDTO);

            if (deletingPlace != null)
            {
                return Ok(new { result = deletingPlace, message = "Sikeres törlés." });
            }
            return NotFound(new { message = "A hely nem található." });
        }
        [Authorize(Roles = "Admin,PlaceOwner,User,Trainer")]
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
        [Authorize(Roles = "Admin,PlaceOwner,User,Trainer")]
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
