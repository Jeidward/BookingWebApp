using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Models.Entities;

namespace MSSQL
{
    public class AmenitiesRepository : Repository
    {
        public AmenitiesRepository(IConfiguration configuration) : base(configuration) { }

        public List<Amenities> GetAmenities()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT * FROM Amenities";
                using (var cmd = new SqlCommand(query, conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        var amenitiesList = new List<Amenities>();
                        while (reader.Read())
                        {
                            var amenities = new Amenities(

                                Convert.ToInt32(reader["AmenitiesId"]),
                                reader["Name"].ToString(),
                                reader["IconPath"].ToString()
                            );

                            amenitiesList.Add(amenities);
                        }
                        return amenitiesList;
                    }
                }
            }
        }
    }
}
