using Models.Entities;
using Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace MSSQL
{
    public class ApartmentRepository : Repository, IApartmentRepository
    {
        public ApartmentRepository(IConfiguration configuration) : base(configuration) { }

        public void CreateApartment(Apartment apartment)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO Apartments (Name, Description, ImageUrl, PricePerNight, Adress, Bedrooms, Bathrooms) 
                                 VALUES (@Name, @Description, @ImageUrl, @PricePerNight, @Adress, @Bedrooms, @Bathrooms)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", apartment.Name);
                    cmd.Parameters.AddWithValue("@Description", apartment.Description);
                    cmd.Parameters.AddWithValue("@ImageUrl", apartment.ImageUrl);
                    cmd.Parameters.AddWithValue("@PricePerNight", apartment.PricePerNight);
                    cmd.Parameters.AddWithValue("@Adress", apartment.Adress);
                    cmd.Parameters.AddWithValue("@Bedrooms", apartment.Bedrooms);
                    cmd.Parameters.AddWithValue("@Bathrooms", apartment.Bathrooms);
                    cmd.ExecuteNonQuery();

                }
            }
        }

        public void Delete(int aptId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"UPDATE Apartments SET IsArchived = 1 WHERE ApartmentId = @ApartmentId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ApartmentId", aptId); 
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddApartmentImages(int aptId, string imgPath)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO ApartmentImages (ApartmentId, ImgPath) 
                                 VALUES (@ApartmentId, @ImgPath)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ApartmentId", aptId);
                    cmd.Parameters.AddWithValue("@ImgPath", imgPath);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public List<Apartment> GetApartments()
        {
            List<Apartment> apartments = new List<Apartment>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = $@"
                    SELECT  ApartmentId, Name, Description, ImageUrl, PricePerNight, Adress, Bedrooms, Bathrooms 
                    FROM Apartments WHERE IsArchived = 0";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            apartments.Add(new Apartment(
                                Convert.ToInt32(reader["ApartmentId"]),
                                Convert.ToString(reader["Name"])!,
                                Convert.ToString(reader["Description"])!,
                                Convert.ToString(reader["ImageUrl"])!,
                                Convert.ToDecimal(reader["PricePerNight"]),
                                Convert.ToString(reader["Adress"])!, 
                                Convert.ToInt32(reader["Bedrooms"]),
                                Convert.ToInt32(reader["Bathrooms"])
                            ));
                        }
                    }
                }
            }

            return apartments;
        }

        public Apartment GetApartment(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = $@"
                    SELECT ApartmentId, Name, Description, ImageUrl, PricePerNight,Adress, Bedrooms, Bathrooms 
                    FROM Apartments WHERE ApartmentId = @Id AND IsArchived = 0";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            return new(
                                Convert.ToInt32(reader["ApartmentId"]),
                                 Convert.ToString(reader["Name"])!,
                                 Convert.ToString(reader["Description"])!,
                                 Convert.ToString(reader["ImageUrl"])!,
                                 Convert.ToDecimal(reader["PricePerNight"]),
                                 Convert.ToString(reader["Adress"])!,
                                Convert.ToInt32(reader["Bedrooms"]),
                                 Convert.ToInt32(reader["Bathrooms"])

                            );
                        }
                        return Apartment.DefaultApartment();
                    }
                }
            }
        } 


        public List<string> GetGallery(int id)
        {
            List<string> apartmentsImages = new();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @" SELECT ImgPath FROM ApartmentImages WHERE ApartmentId = @ApartmentId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ApartmentId", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            apartmentsImages.Add(reader["ImgPath"].ToString()!);
                            //can you this way instead of get ordinal but dont forget to parse it.
                        }
                    }
                }

            }

            return apartmentsImages;
        }

    }
}

