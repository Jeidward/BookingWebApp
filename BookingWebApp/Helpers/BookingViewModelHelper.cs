using BookingWebApp.ViewModels;
using Models.Entities;
using MSSQL;
using Services;

namespace BookingWebApp.Helpers
{
    public static class BookingViewModelHelper
    {
      

        //factory to produce object wihtout us needing to know of how it producing it, almost always static
        public static BookingViewModel ConvertToViewModel(Booking booking)
        {
            BookingViewModel viewModel = new BookingViewModel() {Id = booking.Id, ApartmentId = booking.Apartment.Id, CheckInDate = booking.CheckInDate, CheckOutDate = booking.CheckOutDate, TotalPrice = booking.TotalPrice, Status = booking.Status };

            viewModel.ApartmentViewModel = new ApartmentViewModel
            {
                Id = booking.Apartment.Id,
                Name = booking.Apartment.Name,
                ImageUrl = booking.Apartment.ImageUrl,
                Description = booking.Apartment.Description,
                Adress = booking.Apartment.Adress,
                Gallery = booking.Apartment.Gallery,
                PricePerNight = booking.Apartment.PricePerNight,
                Rating = booking.Apartment.Rating,
                ReviewsCount = booking.Apartment.ReviewsCount
            };
            foreach (var guest in booking.GuestProfiles)
            {
             
                viewModel.GuestProfiles.Add(GuestProfileViewModel.ConvertToViewModel(guest));
            }

            return viewModel;
        }

        public static List<BookingViewModel> ConvertToViewModel(List<Booking> bookings)
        {
            var viewModels = new List<BookingViewModel>();          
            foreach (var booking in bookings)
            {
                viewModels.Add(ConvertToViewModel(booking)); 
            }
            return viewModels;
        }

        public static string CreateString(BookingViewModel bookingViewModel)
        {
            return $"{bookingViewModel.ApartmentId}${bookingViewModel.CheckInDate}${bookingViewModel.CheckOutDate}${bookingViewModel.TotalPrice}";
        }


        public static string CreateString(Booking booking)
        {
            return $"{booking.Id}${booking.Apartment.Id}${booking.CheckInDate}${booking.CheckOutDate}${booking.TotalPrice}";
        }

        public static BookingViewModel ReadModelBooking(string bookingString)
        {
            string[] bookingInfo = bookingString.Split('$');
            BookingViewModel readBooking = new BookingViewModel()
            {
                Id = int.Parse(bookingInfo[0]),
                CheckInDate = DateTime.Parse(bookingInfo[2]),
                CheckOutDate = DateTime.Parse(bookingInfo[3]),
                TotalPrice = decimal.Parse(bookingInfo[4]),
                ApartmentId = int.Parse(bookingInfo[1])
            };
                
            return readBooking;
        }


        public static BookingViewModel ReadString(string bookingString)
        {
            string[] bookingInfo = bookingString.Split('$');
            BookingViewModel readBooking = new BookingViewModel() {
                CheckInDate = DateTime.Parse(bookingInfo[1]),
                CheckOutDate = DateTime.Parse(bookingInfo[2]),
                TotalPrice = decimal.Parse(bookingInfo[3]),
                ApartmentId = int.Parse(bookingInfo[0])
            };
            return readBooking;
        }
    }
    
}
