using Interfaces;
using Enums;
using Models.Entities;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.JavaScript;

namespace Services
{

    public class BookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IApartmentRepository _apartmentRepository;
        private readonly PaymentService _paymentService;   

        public BookingService(IBookingRepository bookingRepository, IApartmentRepository apartmentRepository,PaymentService paymentService)
        {
            _bookingRepository = bookingRepository;
            _apartmentRepository = apartmentRepository;
            _paymentService = paymentService;
        }

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public void FinalizePayment(Booking booking, PaymentMethod paymentMethod)
        {
            Payment payment = _paymentService.CreatePayment(booking, booking.TotalPrice, paymentMethod);
            Payment processPayment = _paymentService.ProcessPayment(payment);
            
            if (processPayment.PaymentStatus == PaymentStatus.SUCCESS)
            {
                booking.SetStatus(BookingStatus.Confirmed);
                this.Update(booking);
            }
            else
            {
                booking.SetStatus(BookingStatus.Pending);
            }
        }


        public Booking GetBookingWithApartment(int bookingId)
        {
            Booking booking = _bookingRepository.GetBookingById(bookingId);

            booking.SetApartment(_apartmentRepository.GetApartment(_bookingRepository.GetApartment(bookingId)));
            booking.SetGuestProfile(_bookingRepository.GetBookingGuests(bookingId));

            return booking;
        }

        public int Save(Booking booking)
        {
            int bookingId = _bookingRepository.SaveBooking(booking);

            booking.SetId(bookingId);
    
            foreach (GuestProfile guest in booking.GuestProfiles)
            {
                _bookingRepository.SaveGuestForBooking(bookingId, guest);
            }

            return bookingId;
        }

        public void Update(Booking booking)
        {
            _bookingRepository.Update(booking);
        }

        public decimal CalculateTotalPrice(DateTime checkInDate, DateTime checkOutDate, Apartment apartment)
        {
            decimal totalPrice = apartment.PricePerNight * ComputeNights(checkInDate, checkOutDate);
            return totalPrice;
        }

        public int ComputeNights(DateTime checkIn, DateTime checkOut)
        {
            return (checkOut - checkIn).Days;
        }


        public List<Booking> GetAllBookingsForUser(int userId)
        {
            List<Booking> userBooking = _bookingRepository.GetBookingsByUserId(userId);

            foreach (Booking booking in userBooking)
            {
                booking.SetApartment(_apartmentRepository.GetApartment(_bookingRepository.GetApartment(booking.Id)));
                booking.SetGuestProfile( _bookingRepository.GetBookingGuests(booking.Id));

            }

            return userBooking;
        }

        public List<Booking> GetAllBookingsForUserCheckout(int userId) => GetAllBookingsForUser(userId)
            .FindAll(booking => booking.Status == BookingStatus.CheckedOut);

        public List<Booking> GetAllBookingForUserCurrent(int userId) => GetAllBookingsForUser(userId)
            .FindAll(booking => booking.Status == BookingStatus.Confirmed);
            
        

        public bool IsOverlappingBookingExist(int apartmentId, DateTime checkInDate, DateTime checkOutDate)
        {
            return _bookingRepository.IsOverlappingBookingExist(apartmentId,checkInDate,checkOutDate);
        }

        public bool CancelBooking(int bookingId,DateTime checkInDate)
        {
            var currentDay = DateTime.Today;
            var period = checkInDate - currentDay;
            var sevenDays = TimeSpan.FromDays(7);
            period = DateTime.Today.TimeOfDay; // for testing, remove after
            if (period > sevenDays)
            {
                _bookingRepository.CancelBooking(bookingId);
                return true;
            }
            else
            { 
                return false;
            }
        }
    }
}
