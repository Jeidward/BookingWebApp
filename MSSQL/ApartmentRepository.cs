using Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Interfaces.IRepositories;

namespace MSSQL
{
    public class ApartmentRepository(IConfiguration configuration) : Repository(configuration), IApartmentRepository
    {
        public void CreateApartment(Apartment apartment)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query =
                    @"INSERT INTO Apartments (Name, Description, PricePerNight, Adress, Bedrooms, Bathrooms,IsArchived) 
                                 VALUES (@Name, @Description, @PricePerNight, @Adress, @Bedrooms, @Bathrooms, @IsArchived)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", apartment.Name);
                    cmd.Parameters.AddWithValue("@Description", apartment.Description);
                    cmd.Parameters.AddWithValue("@PricePerNight", apartment.PricePerNight);
                    cmd.Parameters.AddWithValue("@Adress", apartment.Adress);
                    cmd.Parameters.AddWithValue("@Bedrooms", apartment.Bedrooms);
                    cmd.Parameters.AddWithValue("@Bathrooms", apartment.Bathrooms);
                    cmd.Parameters.AddWithValue("IsArchived", 0);
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
                    SELECT  ApartmentId, Name, Description, PricePerNight, Adress, Bedrooms, Bathrooms 
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
                    SELECT ApartmentId, Name, Description, PricePerNight,Adress, Bedrooms, Bathrooms 
                    FROM Apartments WHERE ApartmentId = @Id AND IsArchived = 0";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new(
                                Convert.ToInt32(reader["ApartmentId"]),
                                Convert.ToString(reader["Name"])!,
                                Convert.ToString(reader["Description"])!,
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
                        }
                    }
                }

            }

            return apartmentsImages;
        }

        public void Update(Apartment apartment)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"UPDATE Apartments 
                                 SET Name = @Name, Description = @Description, PricePerNight = @PricePerNight, Adress = @Adress, Bedrooms = @Bedrooms, Bathrooms = @Bathrooms 
                                 WHERE ApartmentId = @ApartmentId";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ApartmentId", apartment.Id);
                    cmd.Parameters.AddWithValue("@Name", apartment.Name);
                    cmd.Parameters.AddWithValue("@Description", apartment.Description);
                    cmd.Parameters.AddWithValue("@PricePerNight", apartment.PricePerNight);
                    cmd.Parameters.AddWithValue("@Adress", apartment.Adress);
                    cmd.Parameters.AddWithValue("@Bedrooms", apartment.Bedrooms);
                    cmd.Parameters.AddWithValue("@Bathrooms", apartment.Bathrooms);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void UpdateGallery(int id, List<string> gallery)
        {

            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using (var del = new SqlCommand(
                       "DELETE FROM ApartmentImages WHERE ApartmentId = @ApartmentId", conn))
            {
                del.Parameters.AddWithValue("@ApartmentId", id);
                del.ExecuteNonQuery();
            }

            foreach (var imgPath in gallery)
            {
                AddApartmentImages(id, imgPath);
            }
        }

        public async Task<PaginatedList<Apartment>> GetApartmentsAsync(int pageIndex, int pageSize)
        {
            await using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            var total = (int)(await new SqlCommand(
                "SELECT COUNT(*) FROM dbo.Apartments WHERE IsArchived = 0", conn).ExecuteScalarAsync())!;

            string query = $@"
                    SELECT ApartmentId, Name, Description, PricePerNight, Adress, Bedrooms, Bathrooms 
                    FROM Apartments WHERE IsArchived = 0
                    ORDER BY ApartmentId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
            await using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            await using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            var apartments = new List<Apartment>();
            while (await reader.ReadAsync())
            {
                apartments.Add(new Apartment(
                    Convert.ToInt32(reader["ApartmentId"]),
                    Convert.ToString(reader["Name"])!,
                    Convert.ToString(reader["Description"])!,
                    Convert.ToDecimal(reader["PricePerNight"]),
                    Convert.ToString(reader["Adress"])!,
                    Convert.ToInt32(reader["Bedrooms"]),
                    Convert.ToInt32(reader["Bathrooms"])
                ));
            }
            return PaginatedList<Apartment>.Create(apartments,total, pageIndex, pageSize);
        }
    }
}
