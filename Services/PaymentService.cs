using Microsoft.Extensions.Configuration;
using Models.Entities;
using Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.IRepositories;

namespace Services
{
    public class PaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }


        public Payment ProcessPayment(Payment payment)
        {
            if (payment.PaymentStatus != PaymentStatus.SUCCESS)// this will always be true, redundant
            {
                payment.PaymentStatus = PaymentStatus.SUCCESS;
                payment = _paymentRepository.SavePayment(payment);
            }
            else
            {
                throw new Exception("Payment has already been paid!");
            }
            return payment;
        }


        public Payment CreatePayment(Booking booking, decimal amount, PaymentMethod paymentMethod)
        {
            this.ValidateBookingObject(booking);
            int transactionId = new Random().Next(100000, 999999);
            Payment payment = new Payment(booking, amount, paymentMethod, PaymentStatus.UNPAID, transactionId);

            return payment;
        }

        private void ValidateBookingObject(Booking booking)
        {
            if (booking.CheckInDate == DateTime.MinValue) throw new ArgumentException("Check-in date is not set.");
            if (booking.CheckOutDate == DateTime.MinValue) throw new ArgumentException("Check-out date is not set.");
            if (booking.TotalPrice <= 0) throw new ArgumentException("Total price is not set.");
            if (booking.GuestProfiles == null || booking.GuestProfiles.Count == 0) throw new ArgumentException("Guest profiles are not set.");
            if(booking.Status != BookingStatus.Pending) throw new ArgumentException("Booking status is not set.");
            if (booking.Apartment == null) throw new ArgumentException("Apartment cannot be null");
            if (booking.CheckoutReminderSent != false) throw new ArgumentException("checkoutReminderSent should be set to false");
        }
    }
}
