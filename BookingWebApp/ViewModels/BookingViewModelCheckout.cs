using Enums;
using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class BookingViewModelCheckout
    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public int ApartmentId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public ApartmentViewModel ApartmentViewModel { get; set; } // this new


        public static BookingViewModelCheckout ConvertToViewModel(Booking booking)
        {
            BookingViewModelCheckout viewModel = new BookingViewModelCheckout() { Id = booking.Id, ApartmentId = booking.Apartment.Id, CheckInDate = booking.CheckInDate, CheckOutDate = booking.CheckOutDate, TotalPrice = booking.TotalPrice, Status = booking.Status };

            viewModel.ApartmentViewModel = new ApartmentViewModel
            {
                Id = booking.Apartment.Id,
                Name = booking.Apartment.Name,
                ImageUrl = booking.Apartment.ImageUrl,
                Description = booking.Apartment.Description,
                Adress = booking.Apartment.Adress,
                Gallery = booking.Apartment.Gallery,
                PricePerNight = booking.Apartment.PricePerNight,
                AvgRating = booking.Apartment.AvgRating,
                ReviewsCount = booking.Apartment.ReviewsCount
            };

            return viewModel;
        }
        public static List<BookingViewModelCheckout> ConvertToViewModel(List<Booking> bookings)
        {
            var viewModel = new List<BookingViewModelCheckout>();

            foreach (var booking in bookings)
            {
                viewModel.Add(ConvertToViewModel(booking));
            }
            return viewModel;
        }
    }
}
