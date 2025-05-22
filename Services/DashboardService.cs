using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Enums;
using Interfaces.IRepositories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace Services
{
    public class DashboardService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IAccountHolderRepository _accountHolderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IApartmentRepository _apartmentRepository;

        public DashboardService(IBookingRepository bookingRepository, IAccountHolderRepository accountHolderRepository, IPaymentRepository paymentRepository, IApartmentRepository apartmentRepository)
        {
            _bookingRepository = bookingRepository;
            _accountHolderRepository = accountHolderRepository;
            _paymentRepository = paymentRepository;
            _apartmentRepository = apartmentRepository;
        }

        public int GetTotalBookings()
        {
            return _bookingRepository.GetAllBookings();
        }

        public int UpcomingBookings()
        {
            var bookings = _bookingRepository.GetAllBookingsWithObject();
            var upcomingBooking = new List<Booking>();

            foreach (var booking in bookings)
            {
                if (booking.CheckInDate > DateTime.Today)
                    upcomingBooking.Add(booking);
            }
            return upcomingBooking.Count;
        }

        public int GetTotalActiveAccountHolders()
        {
            return _accountHolderRepository.GetTotalAccountHolder();
        }

        public decimal GetTotalRevenue()
        {
            return _paymentRepository.GetTotalRevenue();
        }

        public List<ActivityDashboard> GetAllActivities() => _bookingRepository.GetAllActivitiesObjects();

        public DashboardAnalytics GetDashboardAnalytics()
        {
            var totalBookings = GetTotalBookings();
            var upcomingBookings = UpcomingBookings();
            var totalAccountHolders = GetTotalActiveAccountHolders();
            var totalRevenue = GetTotalRevenue();

            return new DashboardAnalytics(totalBookings, totalAccountHolders, totalRevenue, upcomingBookings);
        }

        //manage apartments section//
        
        public List<Booking> GetOccupiedApartmentFromBookings() =>
            _bookingRepository.GetAllBookingsWithObject().Where(b => DateTime.Today >= b.CheckInDate.Date && DateTime.Today <= b.CheckOutDate.Date && b.Status == BookingStatus.Confirmed).ToList();

        public List<string> AddImage(IFormFile[] gallery,string webRootPath)
        {
            var savedNames = new List<string>();
            foreach (var file in gallery)
            {
                if (file.Length == 0) continue;

                var ext = Path.GetExtension(file.FileName);
                var fileName = Path.GetFileNameWithoutExtension(file.FileName);
                var name = $"{fileName}{ext}";
                var path = Path.Combine(webRootPath, "IMG", name);

                Directory.CreateDirectory(Path.GetDirectoryName(path)!);

                using var stream = File.Create(path);
                file.CopyTo(stream);

                savedNames.Add(name);
            }
            return savedNames;
        }
    }
}
