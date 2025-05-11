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
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public List<string> Gallery { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal AvgRating { get; set; }
        public int ReviewsCount { get; set; }
        public List<ReviewViewModel> ReviewViewModel { get; set; }
        public decimal AverageCleanliness { get; set; }
        public decimal AverageLocation    { get; set; }
        public decimal AverageComfort     { get; set; }
        public decimal AverageValue       { get; set; }
        public CreateBookingViewModel Booking { get; set; }
        public bool IsOccupied { get; set; }// this is for the view to show if the apartment is occupied or not.

        public ApartmentViewModel()
        {
            Gallery = new List<string>();
            ReviewViewModel = new List<ReviewViewModel>();
        }

        
        public static ApartmentViewModel ConvertToViewModel(Apartment apartment)
        {
           ApartmentViewModel apartmentViewModel = new ApartmentViewModel() { Id = apartment.Id, Name = apartment.Name, Description = apartment.Description, ImageUrl = apartment.ImageUrl,Bedrooms = apartment.Bedrooms,Bathrooms = apartment.Bathrooms,Adress = apartment.Adress, Gallery = apartment.Gallery, PricePerNight = apartment.PricePerNight, AvgRating = apartment.AvgRating, ReviewsCount = apartment.ReviewsCount, Booking = new CreateBookingViewModel(){ApartmentId = apartment.Id}, IsOccupied = apartment.IsOccupied};

           if(apartment.Reviews == null)
            {
                apartment.SetReviews( new List<Review>());
            }

            foreach (var review in apartment.Reviews)
            {
                apartmentViewModel.ReviewViewModel.Add(ViewModels.ReviewViewModel.ConvertToViewModel(review));
            }

            if (apartmentViewModel.ReviewViewModel.Any())
            {
                apartmentViewModel.AverageCleanliness =
                    apartmentViewModel.ReviewViewModel.Average(r => r.CleanlinessRating);

                apartmentViewModel.AverageLocation =
                    apartmentViewModel.ReviewViewModel.Average(r => r.LocationRating);

                apartmentViewModel.AverageComfort =
                    apartmentViewModel.ReviewViewModel.Average(r => r.ComfortRating);

                apartmentViewModel.AverageValue =
                    apartmentViewModel.ReviewViewModel.Average(r => r.ValueRating);
            }
            else
            {
                apartmentViewModel.AverageCleanliness = 0;
                apartmentViewModel.AverageLocation = 0;
                apartmentViewModel.AverageComfort = 0;
                apartmentViewModel.AverageValue = 0; 
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
