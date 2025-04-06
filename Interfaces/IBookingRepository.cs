using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IBookingRepository
    {
        public int SaveBooking(Booking booking);
        public int SaveGuestForBooking(int bookingId, GuestProfile guestProfile);
        public List<GuestProfile> GetBookingGuests(int bookingId);
        public int GetApartment(int bookingId);
        public Booking GetBookingById(int bookingId);
        public List<Booking> GetBookingsByUserId(int userId);
        public void Update(Booking  booking);
        public bool IsOverlappingBookingExist(int apartmentId,DateTime checkInDate,DateTime checkOutDate);





    }
}
