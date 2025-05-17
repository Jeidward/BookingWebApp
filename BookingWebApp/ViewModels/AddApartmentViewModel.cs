using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class AddApartmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Adress { get; set; }
        public string ImageUrl { get; set; }
        public List<int> SelectedAmenityIds { get; set; } = new List<int>();// for the create page.
        public List<string>? Gallery { get; set; }
        public decimal CostPerNight { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        public static Apartment ConvertToEntity(AddApartmentViewModel viewModel)
        {
            var amenities =  AmenitiesViewModel.ConvertToEntity(viewModel.SelectedAmenityIds);
            return new Apartment(viewModel.Id,viewModel.Name, viewModel.Description, viewModel.ImageUrl,amenities,viewModel.Gallery ,viewModel.CostPerNight, viewModel.Adress, viewModel.Bedrooms, viewModel.Bathrooms);
        }

        public static AddApartmentViewModel ConvertToViewModel(Apartment apartment)
        {
            return new AddApartmentViewModel
            {
                Id = apartment.Id,
                Name = apartment.Name,
                Description = apartment.Description,
                ImageUrl = apartment.ImageUrl,
                Adress = apartment.Adress,
                CostPerNight = apartment.PricePerNight,
                Bedrooms = apartment.Bedrooms,
                Bathrooms = apartment.Bathrooms,
                Gallery = apartment.Gallery,
                SelectedAmenityIds = apartment.Amenities.Select(a => a.Id).ToList() 
            };
        }

        public static List<AddApartmentViewModel> ConvertToViewModel(List<Apartment> apartments)
        {
            return apartments.Select(ConvertToViewModel).ToList();
        }
    }
    
}
