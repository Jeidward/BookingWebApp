using Models.Entities;

namespace DTOs
{
    public class ApartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Adress { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Gallery { get; set; }
        public List<ReviewDto> Reviews { get;  set; }
        public decimal PricePerNight { get; set; }
        public decimal AvgRating { get;  set; }
        public int ReviewsCount { get;  set; }


        public static ApartmentDto ConvertToDto(Apartment apartment)
        {
           var apartmentDto = new ApartmentDto()
            {
                Id = apartment.Id,
                Name = apartment.Name,
                Description = apartment.Description,
                Adress = apartment.Adress,
                ImageUrl = apartment.ImageUrl,
                PricePerNight = apartment.PricePerNight
            };

            return apartmentDto;
        }
    }
}
