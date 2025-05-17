using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Models.Entities;
using Interfaces.IRepositories;

namespace MSSQL
{
    public class AmenitiesRepository : Repository, IAmenitiesRepository
    {
        public AmenitiesRepository(IConfiguration configuration) : base(configuration) { }

        public List<Amenities> GetAmenities(int id) // for apartment details page
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string query = @"SELECT AAP.Id,AAP.ApartmentId,AAP.AmenityId,AM.Name,AM.ImgIcon FROM Amenities_apartment AS AAP
                                 INNER JOIN Amenities AS AM ON AM.Id = AAP.AmenityId 
                                   WHERE AAP.ApartmentId = @ApartmentId";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("ApartmentId", id);
            using var reader = cmd.ExecuteReader();
            var amenitiesList = new List<Amenities>();
            while (reader.Read())
            {
                var amenities = new Amenities(

                    Convert.ToInt32(reader["Id"]),
                    reader["Name"].ToString()!,
                    reader["ImgIcon"].ToString()!
                );

                amenitiesList.Add(amenities);
            }
            return amenitiesList;
        }

        public List<Amenities> GetAmenitiesList()
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string query = @"SELECT * FROM Amenities";
            using var cmd = new SqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();
            var amenitiesList = new List<Amenities>();
            while (reader.Read())
            {
                var amenities = new Amenities(
                    Convert.ToInt32(reader["Id"]),
                    reader["Name"].ToString()!,
                    reader["ImgIcon"].ToString()!
                );
                amenitiesList.Add(amenities);
            }
            return amenitiesList;
        }

        public void AddAmenities(int apartmentId, int amenityId)
        {
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string query = @"INSERT INTO Amenities_apartment (ApartmentId,AmenityId) VALUES (@ApartmentId,@AmenityId)";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("ApartmentId", apartmentId);
            cmd.Parameters.AddWithValue("AmenityId", amenityId);
            cmd.ExecuteNonQuery();
        }

        public List<Amenities> GetSelectedAmenities(int apartmentId)
        {
            var amenities = new List<Amenities>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string query = @"SELECT AAP.Id,AAP.ApartmentId,AAP.AmenityId,AM.Name,AM.ImgIcon FROM Amenities_apartment AS AAP
                                 INNER JOIN Amenities AS AM ON AM.Id = AAP.AmenityId 
                                   WHERE AAP.ApartmentId = @ApartmentId";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("ApartmentId", apartmentId);
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var amenitiesList = new Amenities(
                    Convert.ToInt32(cmd.ExecuteReader()["Id"]),
                    cmd.ExecuteReader()["Name"].ToString()!,
                    cmd.ExecuteReader()["ImgIcon"].ToString()!
                );
                amenities.Add(amenitiesList);
            }
            return amenities;
        }
    }
}
