using Enums;
using Models.Entities;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.JavaScript;
using Models.Enums;
using Interfaces.IRepositories;

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

        public decimal CalculateTotalPrice(DateTime checkInDate, DateTime checkOutDate, Apartment apartment, List<ExtraService> extraServices)
        {
            var checkDay = DateOnly.FromDateTime(checkInDate);
            var checkoutDay = DateOnly.FromDateTime(checkOutDate);
            var checkInValue = DateTime.Parse(checkDay.ToString());
            var checkOutValue = DateTime.Parse(checkoutDay.ToString());
            decimal totalPrice = apartment.PricePerNight * ComputeNights(checkInValue, checkOutValue);

            foreach (var service in extraServices)
            {
                switch (service)
                {
                    case ExtraService.POOL_RENTAL:
                        totalPrice += 50; 
                        break;
                    case ExtraService.LAUNDRY_RENTAL:
                        totalPrice += 20; 
                        break;
                    case ExtraService.CAR_RENTAL:
                        totalPrice += 100; 
                        break;
                }
            }
            return totalPrice;
        }

        public int ComputeNights(DateTime checkIn, DateTime checkOut)
        {
            var checkInDay = DateOnly.FromDateTime(checkIn);
            var checkOutDay = DateOnly.FromDateTime(checkOut);

            return checkOutDay.Day - checkInDay.Day;
        }


        public List<Booking> GetAllBookingsForUser(int userId)
        {
            List<Booking> userBooking = _bookingRepository.GetBookingsByUserId(userId);

            foreach (Booking booking in userBooking)
            {
                booking.SetApartment(_apartmentRepository.GetApartment(_bookingRepository.GetApartment(booking.ApartmentId)));
                booking.SetGuestProfile( _bookingRepository.GetBookingGuests(booking.Id));
            }

            return userBooking;
        }

        public List<Booking> GetAllBookingsForUserCheckout(int userId)
        {
            var bookings =  GetAllBookingsForUser(userId).FindAll(booking => booking.Status == BookingStatus.CheckedOut);
            foreach (var booking in bookings)
                booking.Apartment.SetFirstImage(_apartmentRepository.GetGallery(booking.Apartment.Id).First());

            return bookings;
        } 

        public List<Booking> GetAllBookingForUserCurrent(int userId)
        {
           var bookings = GetAllBookingsForUser(userId).FindAll(booking => booking.Status == BookingStatus.Confirmed);
           foreach (var booking in bookings)
               booking.Apartment.SetFirstImage(_apartmentRepository.GetGallery(booking.Apartment.Id).First());

           return bookings;
        }
            
        

        public bool IsOverlappingBookingExist(int apartmentId, DateTime checkInDate, DateTime checkOutDate)
        {
            return _bookingRepository.IsOverlappingBookingExist(apartmentId,checkInDate,checkOutDate);
        }

        public bool CancelBooking(int bookingId,DateTime checkInDate)
        {
            var currentDay = DateTime.Today;
            var period = checkInDate - currentDay;
            var sevenDays = TimeSpan.FromDays(7);
            if (period > sevenDays)
            {
                _bookingRepository.CancelBooking(bookingId);
                
                return true;
            } 
            return false;
        }

    }
}
