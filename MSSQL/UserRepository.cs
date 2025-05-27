using System.Runtime.Serialization;
using Interfaces.IRepositories;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class UserRepository : Repository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration) { }

        public bool RegisterUser(User user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO Users (RoleId, FirstName, LastName, Age, PhoneNumber, Country, Address, Email, Password, IsArchived, Salt)
                    OUTPUT INSERTED.UserId
                    VALUES (@RoleId, @FirstName, @LastName, @Age, @PhoneNumber, @Country, @Address, @Email, @Password, @IsArchived, @Salt)";
                int newUserId;
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@RoleId", 1);
                    cmd.Parameters.AddWithValue("@FirstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", user.LastName);
                    cmd.Parameters.AddWithValue("@Age", user.Age);
                    cmd.Parameters.AddWithValue("@PhoneNumber", user.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Country", user.Country);
                    cmd.Parameters.AddWithValue("@Address", user.Address);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@IsArchived", 0);
                    cmd.Parameters.AddWithValue("@Salt", user.Salt);

                    newUserId = (int)cmd.ExecuteScalar();
                }


                string insertAHQuery = @"
                INSERT INTO AccountHolders (UserId, IsArchived)
                VALUES (@UserId, 0)";
                using (SqlCommand cmd2 = new SqlCommand(insertAHQuery, conn))
                {
                    cmd2.Parameters.AddWithValue("@UserId", newUserId);
                    cmd2.ExecuteNonQuery();
                }

                return true;
            }
        }


        public bool DoesUserExist(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM [Users] WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public int LogIn(string email, string password)
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                //here i check if the password is the same
                string query = @" SELECT UserId, FirstName FROM Users WHERE Email = @Email AND [Password] = @Password;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);

                    object result = cmd.ExecuteScalar();
                    if (result == null)
                    {
                        return 0;
                    }

                    int userId = Convert.ToInt32(result);

                    return userId;
                }
            }
        }


        public User GetUser(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT [UserId],[Email],[Password],[IsArchived],[FirstName],[Salt],[RoleId] FROM Users WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new(
                                Convert.ToInt32(reader["UserId"]),
                                Convert.ToString(reader["Email"])!,
                                Convert.ToString(reader["Password"])!,
                                Convert.ToString(reader["FirstName"])!,
                                Convert.ToString(reader["Salt"])!,
                                Convert.ToInt32(reader["RoleId"])
                            );
                            return user;
                        }
                    }
                }
                return new( -1);
            }
        }
        public User GetUser(int Id) 
        {
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT [UserId],RoleId,FirstName, LastName, Age, PhoneNumber, Country, Address, Email FROM Users WHERE UserId = @UserId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", Id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new(

                                Convert.ToInt32(reader["UserId"]),
                                Convert.ToInt32(reader["RoleId"]),
                                Convert.ToString(reader["FirstName"])!,
                                Convert.ToString(reader["LastName"]),
                                Convert.ToInt32(reader["Age"]),
                                Convert.ToString(reader["PhoneNumber"]),
                                Convert.ToString(reader["Country"]),
                                Convert.ToString(reader["Address"]),
                                Convert.ToString(reader["Email"])!

                            );

                            return user;    
                        }
                    }
                }

                return new User(-1);
            }
        }
    }


}
