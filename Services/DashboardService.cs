using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

namespace Services
{
    public class DashboardService
    {
        private readonly IBookingRepository _bookingRepository;
        public DashboardService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }
        public int GetTotalBookings()
        {
            return _bookingRepository.GetAllBookings();
        }

        //public int GetTotalActiveAccountHolders()
        //{

        //};
    }
}
