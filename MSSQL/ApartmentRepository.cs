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

        public List<Apartment> GetApartments(int count)
        {
            List<Apartment> apartments = new List<Apartment>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = $@"
                    SELECT TOP {count} ApartmentId, Name, Description, ImageUrl, PricePerNight, Adress 
                    FROM Apartments";

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
                                Convert.ToString(reader["Adress"])!
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
                    SELECT ApartmentId, Name, Description, ImageUrl, PricePerNight,Adress 
                    FROM Apartments WHERE ApartmentId = @Id";

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
                                 Convert.ToString(reader["Adress"])!
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

