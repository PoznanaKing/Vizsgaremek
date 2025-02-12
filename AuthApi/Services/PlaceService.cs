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

        public async Task<PlaceTable> EditPlace(EditPlaceDTO editPlaceDTO)
        {
            var editingPlace = await _appDbContext.places.FirstOrDefaultAsync(x=>x.PlaceId==editPlaceDTO.placeid);
            if (editingPlace != null)
            {
                return editingPlace;
            }
            return null;
        }

        public Task<PlaceTable> UploadPlace(UploadPlaceDTO uploadPlaceDTO)
        {
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
            if (newPlace != null)
            {
                return Task.FromResult(newPlace);
            }
            return null;
        }
    }
}
