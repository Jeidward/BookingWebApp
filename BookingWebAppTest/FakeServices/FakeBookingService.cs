using BookingWebAppTest.NewFolder;
using Enums;
using Interfaces;
using Models.Entities;

namespace BookingWebAppTest.FakeServices
{
    public class FakeBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly FakePaymentService _paymentService;

        public FakeBookingService(IBookingRepository bookingRepository, FakePaymentService paymentService)
        {
            _bookingRepository = bookingRepository;
            _paymentService = paymentService;
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

      
        public void FinalizePayment(int bookingId, Booking booking, PaymentMethod paymentMethod)
        {
            booking.SetId(bookingId);

            Payment payment = _paymentService.CreatePayment(booking, booking.TotalPrice, paymentMethod);

   
            Payment processedPayment = _paymentService.ProcessPayment(payment);


            if (processedPayment.PaymentStatus == PaymentStatus.SUCCESS)
            {
                booking.SetStatus(BookingStatus.Confirmed);
            }
            else
            {
                booking.SetStatus(BookingStatus.Pending); 
            }
        }
    }
}
