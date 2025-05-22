using Enums;

namespace BookingWebApp.ViewModels
{
    public class IndexViewModel
    {
        public List<BookingViewModel> BookingViewModel { get; set; }
        public UserViewModel UserViewModel { get; set; }
        public BookingViewModel CurrentBooking => BookingViewModel.Find(b=> b.CheckOutDate.Date == DateTime.Today && b.Status == BookingStatus.Confirmed);
        public List<ReviewViewModel> ReviewViewModel { get; set; }
        public int NumberOfCheckout => BookingViewModel.Count(b=> b.CheckOutDate.Date == DateTime.Today && b.Status == BookingStatus.Confirmed);


    }
}
