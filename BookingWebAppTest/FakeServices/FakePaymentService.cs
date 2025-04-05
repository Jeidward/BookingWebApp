using Enums;
using Models.Entities;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookingWebAppTest.NewFolder
{
    public class FakePaymentService
    {
        // This fake implementation doesn't depend on any external repository.
        public  Payment CreatePayment(Booking booking, decimal amount, PaymentMethod paymentMethod)
        {
            int transactionId = 123456;

            Payment payment = new Payment(booking, amount, paymentMethod, PaymentStatus.UNPAID, transactionId);
            return payment;
        }

        public Payment ProcessPayment(Payment payment)
        {
            if (payment.PaymentStatus != PaymentStatus.SUCCESS)
            {
                payment.PaymentStatus = PaymentStatus.SUCCESS;
            }
            else
            {
                throw new Exception("Payment has already been paid!");
            }
            return payment;
        }

    }
}
