namespace AuthApi.Models.Dtos
{
    public record UploadPlaceDTO(string placename, int postalcode, string townname,string streetname, int storylevel, string description, double rating);

    public record EditPlaceDTO(string placename, int postalcode, string townname, string streetname, int storylevel, string description, double rating, int placeid);

    public class DeletePlaceDTO
    {
        public int placeid { get; set; }
    }
    public record GetPlaceById(int placeid);











}
