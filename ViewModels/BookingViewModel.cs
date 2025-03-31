using Enums;
using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class BookingViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ApartmentId { get; set; }
        public ApartmentViewModel ApartmentViewModel { get; set; }
        public List<GuestProfileViewModel> GuestProfiles { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }

        public BookingViewModel()
        {
            this.GuestProfiles = new List<GuestProfileViewModel>();
        }


    }
}
