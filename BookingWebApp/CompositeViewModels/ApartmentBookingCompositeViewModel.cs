using BookingWebApp.ViewModels;

namespace BookingWebApp.CompositeViewModels
{
    public class ApartmentBookingCompositeViewModel
    {
        public List<BookingViewModel> BookingViewModel { get; set; }
        public List<BookingViewModelCheckout> BookingViewModelCheckouts { get; set; } 
        
        public ApartmentBookingCompositeViewModel()
        {
            this.BookingViewModel = new List<BookingViewModel>();   
            this.BookingViewModelCheckouts = new List<BookingViewModelCheckout>();
        }
    }
}
