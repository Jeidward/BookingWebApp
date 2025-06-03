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
        private readonly BookingService _bookingService;

        public DashboardService(IBookingRepository bookingRepository, IAccountHolderRepository accountHolderRepository, IPaymentRepository paymentRepository, IApartmentRepository apartmentRepository, BookingService bookingService)
        {
            _bookingRepository = bookingRepository;
            _accountHolderRepository = accountHolderRepository;
            _paymentRepository = paymentRepository;
            _apartmentRepository = apartmentRepository;
            _bookingService = bookingService;
        }

        public int GetTotalBookings(int selectedMonth,int year) => _bookingRepository.GetAllBookings(selectedMonth,year);
        public List<Booking> UpcomingBookings()
        { 
            var bookings = _bookingRepository.GetAllBookingsWithObject();
            var upcomingBooking = new List<Booking>();

            foreach (var booking in bookings)
            {
                booking.SetGuestProfile(_bookingRepository.GetBookingGuests(booking.Id));
                booking.SetApartment(_apartmentRepository.GetApartment(booking.ApartmentId));

                if (booking.CheckInDate > DateTime.Today)
                    upcomingBooking.Add(booking);
            }

            return upcomingBooking;
        }

        public int GetTotalActiveAccountHolders()
        {
            return _accountHolderRepository.GetTotalAccountHolder();
        }


        public List<Booking> CurrentStaying(List<User> users)
        {
            var currentStaying = new List<Booking>();

            foreach (var user in users)
            {
                var bookings = _bookingService.GetAllBookingsForUser(user.Id);
                currentStaying.AddRange(bookings.Where(b => DateTime.Today >= b.CheckInDate.Date && DateTime.Today <= b.CheckOutDate.Date && b.Status == BookingStatus.Confirmed));

            }

            return currentStaying;
        }

        public decimal GetTotalRevenue(int selectedMonth,int year)=> _paymentRepository.GetTotalRevenue(selectedMonth, year);

        public List<ActivityDashboard> GetAllActivities() => _bookingRepository.GetAllActivitiesObjects(); // this can probably be removed
        public async Task<PaginatedList<ActivityDashboard>> GetAllActivitiesAsync(int pageIndex, int pageSize) =>
            await _bookingRepository.GetAllActivitiesObjectAsync(pageIndex, pageSize);

        public DashboardAnalytics GetDashboardAnalytics(int selectedMonth, int year)
        {
            var totalBookings = GetTotalBookings(selectedMonth, year);
            var upcomingBookings = UpcomingBookings();
            var totalAccountHolders = GetTotalActiveAccountHolders();
            var totalRevenue = GetTotalRevenue(selectedMonth, year);

            return new DashboardAnalytics(totalBookings, totalAccountHolders, totalRevenue, upcomingBookings.Count);
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
