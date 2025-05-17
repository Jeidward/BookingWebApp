using Enums;

namespace Models.Entities
{
    public class Payment
    {
        //public int Id { get; }
        public Booking Booking { get; }
        public decimal Amount { get; }
        public PaymentMethod PaymentMethods { get; }
        public PaymentStatus PaymentStatus { get; set; }
        public int TransactionId { get; }

        public Payment(Booking booking, decimal amount, PaymentMethod paymentMethod, PaymentStatus paymentStatus, int transactionId)
        {

            Booking = booking;
            Amount = amount;
            PaymentMethods = paymentMethod;
            TransactionId = transactionId;
            PaymentStatus = paymentStatus;
        }

     

    }
}
