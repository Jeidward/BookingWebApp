using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enums;
using Models.Entities;

namespace Services
{
    public class CheckOutService
    {
        private readonly BookingService _bookingService;
        public CheckOutService(BookingService bookingService)
        {
            _bookingService = bookingService;
        }
        public void ProcessCheckOut(int bookingId)
        {
            Booking booking = _bookingService.GetBookingWithApartment(bookingId);
            
            if (booking.CheckOutDate.Date == DateTime.Today.AddDays(2)) // for test, will be remove
            {
                booking.SetStatus(BookingStatus.CheckedOut);
                _bookingService.Update(booking);
            }
            else
            {
                throw new InvalidOperationException("Cannot process checkout");
            }
        }
    }
}
