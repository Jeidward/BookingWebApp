using Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;

namespace MSSQL
{
    public class UserRepository : Repository, IUserRepository
    {
        public UserRepository(IConfiguration configuration) : base(configuration) { }


        public bool RegisterUser(string email, string password, string name, string salt)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"
                    INSERT INTO Users (Email, [Password],[isArchived],Name, Salt)
                    OUTPUT INSERTED.UserId
                    VALUES (@Email, @Password, 0,@Name, @Salt)";
                int newUserId;
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Name", name);
                    cmd.Parameters.AddWithValue("@Salt", salt);
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
                string query = @" SELECT UserId, Name FROM Users WHERE Email = @Email AND [Password] = @Password;";

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
                string query = @"SELECT [UserId],[Email],[Password],[IsArchived],[Name],[Salt] FROM Users WHERE Email = @Email";
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
                                Convert.ToString(reader["Name"])!,
                                Convert.ToString(reader["Salt"])!
                            );
                            return user;
                        }
                    }
                }
                return new( "", "", "");
            }
        }
        public User GetUser(int Id) 
        {
            using(SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT [UserId],[Email],[Password],[IsArchived],[Name] FROM Users WHERE UserId = @UserId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", Id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User user = new(

                                Convert.ToInt32(reader["UserId"]),
                                Convert.ToString(reader["Email"])!,
                                Convert.ToString(reader["Password"])!,
                                Convert.ToString(reader["Name"])!
                            );

                            return user;    
                        }
                    }
                }
                return new("","","") ;
            }
        }
    }


}
