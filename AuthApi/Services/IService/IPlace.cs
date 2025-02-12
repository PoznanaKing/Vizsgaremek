using AuthApi.Models;
using AuthApi.Models.Dtos;

namespace AuthApi.Services.IService
{
    public interface IPlace
    {
        public Task<PlaceTable> UploadPlace(UploadPlaceDTO uploadPlaceDTO);
        public Task<PlaceTable> EditPlace(EditPlaceDTO editPlaceDTO);
    }
}
