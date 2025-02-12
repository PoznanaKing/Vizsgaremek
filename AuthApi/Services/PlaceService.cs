using AuthApi.Models;
using AuthApi.Models.Dtos;
using AuthApi.Services.IService;

namespace AuthApi.Services
{
    public class PlaceService : IPlace
    {
        private readonly AppDbContext _appDbContext;

        public PlaceService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
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
