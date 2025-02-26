using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services.IService;
using Microsoft.EntityFrameworkCore;

namespace AuthApi.Services
{
    public class PlaceService : IPlace
    {
        private readonly AppDbContext _appDbContext;

        public PlaceService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<PlaceTable> DeletePlace(DeletePlaceDTO deletePlaceDTO)
        {
            var place = await _appDbContext.places.FindAsync(deletePlaceDTO.placeid);
            if (place != null)
            {
                _appDbContext.places.Remove(place);
                await _appDbContext.SaveChangesAsync();
                return place;
            }
            return null;
        }

        public async Task<PlaceTable> EditPlace(EditPlaceDTO editPlaceDTO)
        {
            if (editPlaceDTO == null)
            {
                return null;
            }

            var editingPlace = await _appDbContext.places
                .FirstOrDefaultAsync(x => x.PlaceId == editPlaceDTO.placeid);

            if (editingPlace != null)
            {
                editingPlace.PlaceName = editPlaceDTO.placename;
                editingPlace.TownName = editPlaceDTO.townname;
                editingPlace.Description = editPlaceDTO.description;
                editingPlace.PostalCode = editPlaceDTO.postalcode;
                editingPlace.Rating = editPlaceDTO.rating;
                editingPlace.StoryLevel = editPlaceDTO.storylevel;
                editingPlace.StreetName = editPlaceDTO.streetname;

                await _appDbContext.SaveChangesAsync();
            }

            return editingPlace;
        }

        public async Task<PlaceTable> GetPlaceById(GetPlaceById getPlaceById)
        {
            var placeById = await _appDbContext.places.FirstOrDefaultAsync(x => x.PlaceId == getPlaceById.placeid);
            if (placeById != null) {
                return placeById;
            }
            return null;
                
                
                
        }

        public Task<PlaceTable> UploadPlace(UploadPlaceDTO uploadPlaceDTO)
        {
            if (uploadPlaceDTO == null)
            {
                return Task.FromResult<PlaceTable>(null);
            }

            var newPlace = new PlaceTable
            {
                PlaceName = uploadPlaceDTO.placename,
                TownName = uploadPlaceDTO.townname,
                Description = uploadPlaceDTO.description,
                PostalCode = uploadPlaceDTO.postalcode,
                Rating = uploadPlaceDTO.rating,
                StoryLevel = uploadPlaceDTO.storylevel,
                StreetName = uploadPlaceDTO.streetname,
            };

            return Task.FromResult(newPlace);
        }
    }
}
