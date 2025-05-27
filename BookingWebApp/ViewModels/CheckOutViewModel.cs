using System.ComponentModel.DataAnnotations;

namespace BookingWebApp.ViewModels
{
    public class CheckOutViewModel
    {
        public BookingViewModel BookingViewModel { get; set; }
        public int TotalNights { get; set; }

       
    }
}
