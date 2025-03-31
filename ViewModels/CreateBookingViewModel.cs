using System.ComponentModel.DataAnnotations;

namespace BookingWebApp.ViewModels
{
    public class CreateBookingViewModel
    {
        public int NumberOfGuests { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }

        [DataType(DataType.Time)]
        public DateTime CheckInTime { get; set; }

        [DataType(DataType.Time)]
        public DateTime CheckOutTime { get; set; }

        public int ApartmentId { get; set; }
    }
}
