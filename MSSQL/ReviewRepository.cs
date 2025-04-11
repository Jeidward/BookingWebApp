using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Models.Entities;


namespace MSSQL
{
    public class ReviewRepository : Repository,IReviewRepository
    {
        public ReviewRepository(IConfiguration configuration) : base(configuration){}
        
        public int Save(int apartmentId,Review review)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();    
                
                string query = "INSERT INTO Reviews (ApartmentId,AccountHolderId, Rating, Comment, CreatedAt, IsArchived ,CleanlinessRating, LocationRating, ComfortRating, ValueRating) VALUES (@ApartmentId, @AccountHolderId, @Rating, @Comment, @CreatedAt, @IsArchived,@CleanlinessRating, @LocationRating, @ComfortRating, @ValueRating)";
                
                SqlCommand cmd = new SqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);
                cmd.Parameters.AddWithValue("@AccountHolderId", review.Account.Id);
                cmd.Parameters.AddWithValue("@Rating", review.Rating);
                cmd.Parameters.AddWithValue("@Comment", review.Comment);
                cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                cmd.Parameters.AddWithValue("@IsArchived", 0);
                cmd.Parameters.AddWithValue("@CleanlinessRating", review.CleanlinessRating);
                cmd.Parameters.AddWithValue("LocationRating", review.LocationRating);
                cmd.Parameters.AddWithValue("@ComfortRating", review.ComfortRating);
                cmd.Parameters.AddWithValue("@ValueRating", review.ValueRating);
               
                int newReview = cmd.ExecuteNonQuery();
                return newReview;
            }
        }

        public List<Review>? GetReviewsApartment(int apartmentId)
        {
            List<Review> reviews = new List<Review>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT AccountHolderId,Rating, Comment,CreatedAt,IsArchived,CleanlinessRating,LocationRating,ComfortRating,ValueRating FROM Reviews WHERE ApartmentId = @ApartmentId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows) 
                        {
                            return null; 
                        }

                        while (reader.Read())
                        {
                             reviews.Add(new(

                             new AccountHolder(Convert.ToInt32(reader["AccountHolderId"])),
                              Convert.ToInt32(reader["Rating"]), 
                              reader["Comment"].ToString(),
                              Convert.ToInt32(reader["IsArchived"]), 
                              Convert.ToInt32(reader["CleanlinessRating"]),
                              Convert.ToInt32(reader["LocationRating"]),
                              Convert.ToInt32(reader["ComfortRating"]),
                              Convert.ToInt32(reader["ValueRating"]),
                              Convert.ToDateTime(reader["CreatedAt"])

                         ));
                        }
                       
                    }
                }
            }

            return reviews;
        }
    }
}
