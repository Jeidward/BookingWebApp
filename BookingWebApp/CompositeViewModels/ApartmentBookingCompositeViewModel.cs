using BookingWebApp.ViewModels;

namespace BookingWebApp.CompositeViewModels
{
    public class ApartmentBookingCompositeViewModel
    {
        public List<BookingViewModel> BookingViewModel { get; set; }
        //public ApartmentViewModel ApartmentViewModel { get; set; }

        public ApartmentBookingCompositeViewModel()
        {
            this.BookingViewModel = new List<BookingViewModel>();   
        }
    }
}
