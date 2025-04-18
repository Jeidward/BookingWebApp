﻿using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Interfaces;
using Models.Entities;

namespace MSSQL
{
    public class PaymentRepository : Repository, IPaymentRepository
    {
        public PaymentRepository(IConfiguration configuration) : base(configuration) { }

        public Payment SavePayment(Payment payment)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    string query = @"INSERT INTO Payments (BookingId, Amount, PaymentMethod, Status, TransactionId)
                    VALUES (@BookingId, @Amount, @PaymentMethod, @Status, @TransactionId)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BookingId", payment.Booking.Id);
                        cmd.Parameters.AddWithValue("@Amount", payment.Amount);
                        cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethods.ToString());
                        cmd.Parameters.AddWithValue("@Status", payment.PaymentStatus.ToString());
                        cmd.Parameters.AddWithValue(@"TransactionId", payment.TransactionId);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception("" + ex.Message);
                }
            }

            // Return the payment object after saving it
            return payment;
        }


    }
}
