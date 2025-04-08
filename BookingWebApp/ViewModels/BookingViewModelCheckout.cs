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


        public static BookingViewModelCheckout ConvertToViewModel(Booking booking)
        {
            BookingViewModelCheckout viewModel = new BookingViewModelCheckout() { Id = booking.Id, ApartmentId = booking.Apartment.Id, CheckInDate = booking.CheckInDate, CheckOutDate = booking.CheckOutDate, TotalPrice = booking.TotalPrice, Status = booking.Status };

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
