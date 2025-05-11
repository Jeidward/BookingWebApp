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

        public List<int> Amenities { get; set; }
        public List<string> Gallery { get; set; }
        public decimal CostPerNight { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }

        public static Apartment ConvertToEntity(AddApartmentViewModel viewModel)
        {
            return new Apartment(viewModel.Id, viewModel.Name, viewModel.Description, viewModel.ImageUrl,viewModel.Gallery ,viewModel.CostPerNight, viewModel.Adress, viewModel.Bedrooms, viewModel.Bathrooms);
        }
    }
    
}
