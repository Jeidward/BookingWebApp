using Microsoft.Data.SqlClient;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Interfaces;
using Models.Entities;
using Enums;


namespace MSSQL
{ //hello testing
    public class AccountHolderRepository : Repository,  IAccountHolderRepository
    {
        public AccountHolderRepository(IConfiguration configuration) : base(configuration) { }
        public void SaveAccount(AccountHolder accountHolder)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO AccountHolders
                               ([UserId])VALUES (@UserId)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", accountHolder.Id);
                }

            }
        }

        public AccountHolder GetAccountHolder(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT AccountHolderId FROM AccountHolders WHERE AccountHolderId = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        {
                            return new AccountHolder(

                                Convert.ToInt32(reader["AccountHolderId"])

                            );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// return accountholder based on user id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns> an accountHolder.Id == -1 then accountHolder doesn't exist.</returns>
        public AccountHolder GetAccountHolderByUserId(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT AccountHolderId FROM AccountHolders WHERE UserId = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        reader.Read();
                        {
                            return new AccountHolder(

                                Convert.ToInt32(reader["AccountHolderId"])

                            );
                        }
                    }
                }
            }
        }

        public bool HasBookingForAccountHolder(int accountId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                            SELECT * FROM Bookings b
                            JOIN AccountHolders a ON b.AccountHolderId = a.AccountHolderId
                            WHERE a.UserId = @userId AND b.Status = 'Confirmed'";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("userId", accountId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            return true;

                        }
                        return false;
                    }
                }
            }
        }
    }
}
