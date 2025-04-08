using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class ApartmentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Adress { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Gallery { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal Rating { get; set; }
        public int ReviewsCount { get; set; }
        public List<ReviewViewModel> ReviewViewModel { get; set; }
        public CreateBookingViewModel Booking { get; set; }

        public ApartmentViewModel()
        {
            Gallery = new List<string>();
            ReviewViewModel = new List<ReviewViewModel>();
        }

        public Apartment ConvertToModel()
        {
            Apartment apartment = new Apartment(Id, Name, Description, ImageUrl, PricePerNight, Rating, ReviewsCount, Adress);
            apartment.SetGallery(Gallery);
            return apartment;
        }

        public static ApartmentViewModel ConvertToViewModel(Apartment apartment)
        {
           ApartmentViewModel apartmentViewModel = new ApartmentViewModel() { Id = apartment.Id, Name = apartment.Name, Description = apartment.Description, ImageUrl = apartment.ImageUrl, Adress = apartment.Adress, Gallery = apartment.Gallery, PricePerNight = apartment.PricePerNight, Rating = apartment.Rating, ReviewsCount = apartment.ReviewsCount, Booking = new CreateBookingViewModel(){ApartmentId = apartment.Id} };

            foreach (var review in apartment.Reviews)
            {
                apartmentViewModel.ReviewViewModel.Add(ViewModels.ReviewViewModel.ConvertToViewModel(review));
            }
            return apartmentViewModel;
        }
        public static List<ApartmentViewModel> ConvertToViewModel(List<Apartment> apartments) // apartment list page.
        {
            var viewModels = new List<ApartmentViewModel>();
            {
                foreach (var apartment in apartments)
                {
                    {
                        viewModels.Add(ConvertToViewModel(apartment));
                    }
                }
                return viewModels;
            }
        }
    }
}
