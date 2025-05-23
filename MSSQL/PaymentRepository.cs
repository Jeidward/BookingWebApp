using Enums;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;
using Interfaces.IRepositories;

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

        public decimal GetTotalRevenue(int selectedMonth,int year)
        {
            decimal totalRevenue = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    string query = "SELECT SUM(p.[Amount]) FROM Payments AS p " +
                                   "INNER JOIN Bookings AS b ON b.BookingId = p.BookingId " +
                                   "WHERE p.Status = @Status AND MONTH(b.BookingCreated) = @BookingCreated AND YEAR(b.BookingCreated) = @Year";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Status", PaymentStatus.SUCCESS.ToString());
                        cmd.Parameters.AddWithValue("@BookingCreated", selectedMonth);
                        cmd.Parameters.AddWithValue("@Year", year);
                        object result = cmd.ExecuteScalar();
                        if (result != DBNull.Value)
                        {
                            totalRevenue = Convert.ToDecimal(result);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("" + ex.Message);
                }
            }
            return totalRevenue;
        }




    }
}
