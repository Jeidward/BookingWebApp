using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices.JavaScript;

namespace BookingWebApp.ViewModels
{
    public class CreateBookingViewModel
    {
        public int NumberOfGuests { get; set; }

        [DataType(DataType.Date)]
        [IsOverlappingBookingExist]
        public DateTime CheckInDate { get; set; }
       
        [DateRange]
        public DateTime CheckOutDate { get; set; }

        [DataType(DataType.Time)]
        public DateTime CheckInTime { get; set; }

        [DataType(DataType.Time)]
        public DateTime CheckOutTime { get; set; }

        public int ApartmentId { get; set; }

        public CreateBookingViewModel()
        {
            CheckInDate = DateTime.Today;
            CheckOutDate = DateTime.Today.AddDays(1);

            CheckInTime = DateTime.Today.AddHours(15); 
            CheckOutTime = DateTime.Today.AddHours(11); 

     
            NumberOfGuests = 1;
        }
      
    }
}
