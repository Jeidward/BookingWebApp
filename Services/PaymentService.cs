using Microsoft.Extensions.Configuration;
using Models.Entities;
using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;

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
            if (payment.PaymentStatus != PaymentStatus.SUCCESS)
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
            int transactionId = new Random().Next(100000, 999999);
            Payment payment = new Payment(booking, amount, paymentMethod, PaymentStatus.UNPAID, transactionId);

            return payment;
        }

        
    }
}
