using Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Interfaces;
using Enums;
using System.Diagnostics.CodeAnalysis;


namespace MSSQL
{
    public class BookingRepository : Repository, IBookingRepository
    {
        private readonly AccountHolderRepository _accountHolderRepository;
        public BookingRepository(IConfiguration configuration) : base(configuration)
        {
            _accountHolderRepository = new AccountHolderRepository(configuration);
        }

        public int SaveBooking(Booking booking)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    string insertBookingSql = @"
                     INSERT INTO Bookings
                     (ApartmentId, AccountHolderId, CheckInDate, CheckOutDate, TotalPrice, Status)
                     OUTPUT INSERTED.BookingId
                    VALUES (@ApartmentId, @AccountHolderId, @CheckInDate, @CheckOutDate, @TotalPrice, @Status)";

                    using (SqlCommand bookingCmd = new SqlCommand(insertBookingSql, conn))
                    {
                        bookingCmd.Parameters.AddWithValue("@ApartmentId", booking.Apartment.Id);
                        bookingCmd.Parameters.AddWithValue("@AccountHolderId", booking.GuestProfiles[0].Account.Id);
                        bookingCmd.Parameters.AddWithValue("@CheckInDate", booking.CheckInDate);
                        bookingCmd.Parameters.AddWithValue("@CheckOutDate", booking.CheckOutDate);
                        bookingCmd.Parameters.AddWithValue("@TotalPrice", booking.TotalPrice);
                        bookingCmd.Parameters.AddWithValue("@Status", booking.Status.ToString());



                        int newBookingId = (int)bookingCmd.ExecuteScalar();
                        return newBookingId;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Error saving booking: " + ex.Message);
                }
            }
        }

        public int SaveGuestForBooking(int bookingId, GuestProfile guestProfile)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    // 1. Insert into GuestProfiles
                    string insertGuestProfileSql = @"
                    INSERT INTO GuestProfiles 
                    (AccountHolderId, FirstName, LastName, Age, Email, PhoneNumber, Country, Address)
                    OUTPUT INSERTED.GuestProfileId
                    VALUES (@AccountHolderId, @FirstName, @LastName, @Age, @Email, @PhoneNumber, @Country, @Address)";

                    int newGuestProfileId;
                    using (SqlCommand guestCmd = new SqlCommand(insertGuestProfileSql, conn))
                    {
                        guestCmd.Parameters.AddWithValue("@AccountHolderId", guestProfile.Account.Id);
                        guestCmd.Parameters.AddWithValue("@FirstName", guestProfile.FirstName);
                        guestCmd.Parameters.AddWithValue("@LastName", guestProfile.LastName);
                        guestCmd.Parameters.AddWithValue("@Age", guestProfile.Age);
                        guestCmd.Parameters.AddWithValue("@Email", guestProfile.Email);
                        guestCmd.Parameters.AddWithValue("@PhoneNumber", guestProfile.PhoneNumber);
                        //guestCmd.Parameters.AddWithValue("@DateOfBirth", guestProfile.DateOfBirth);
                        guestCmd.Parameters.AddWithValue("@Country", guestProfile.Country);
                        guestCmd.Parameters.AddWithValue("@Address", guestProfile.Address);

                        newGuestProfileId = (int)guestCmd.ExecuteScalar();
                    }

                    // 2. Insert into BookingGuests
                    string insertBookingGuestSql = @"
                    INSERT INTO BookingGuests (BookingId, GuestProfileId, IsArchived)
                    VALUES (@BookingId, @GuestProfileId, @IsArchived)";

                    using (SqlCommand linkCmd = new SqlCommand(insertBookingGuestSql, conn))
                    {
                        linkCmd.Parameters.AddWithValue("@BookingId", bookingId);
                        linkCmd.Parameters.AddWithValue("@GuestProfileId", newGuestProfileId);
                        linkCmd.Parameters.AddWithValue("@IsArchived", false);

                        linkCmd.ExecuteNonQuery();
                    }

                    return newGuestProfileId;
                }
                catch (Exception ex)
                {
                    throw new Exception("Error saving guest or booking guest: " + ex.Message);
                }
            }
        }
        private void CreateAccountHolderToBooking(int userId, int bookingId)//dont firget about this   
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    string query = @"INSERT INTO AccountHolderToBooking (UserId, BookingId)
                    VALUES(@UserId, @BookingId)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserId", userId);
                        cmd.Parameters.AddWithValue("@BookingId", bookingId);

                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {

                    throw new Exception("" + ex.Message);
                }
            }
        }

        public List<GuestProfile> GetBookingGuests(int bookingId)
        {
            List<GuestProfile> guests = new List<GuestProfile>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                try
                {

                    string query = @"
                        SELECT gp.GuestProfileId, gp.AccountHolderId, gp.FirstName, gp.LastName, gp.Age, gp.Email, 
                        gp.PhoneNumber, gp.Country, gp.Address
                        FROM GuestProfiles AS gp
                        INNER JOIN BookingGuests AS bg ON gp.GuestProfileId = bg.GuestProfileId
                        WHERE bg.BookingId = @BookingId 
                        AND bg.IsArchived = 0 ";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@BookingId", bookingId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {

                                int accountHolderId = int.Parse(reader["AccountHolderId"].ToString()!);
                                AccountHolder accountHolder = _accountHolderRepository.GetAccountHolder(accountHolderId); // need to check this

                                GuestProfile guest = new GuestProfile(
                                    accountHolder,
                                    Convert.ToString(reader["FirstName"])!,
                                    Convert.ToString(reader["LastName"])!,
                                    Convert.ToInt32(reader["Age"]),
                                    Convert.ToString(reader["Email"])!,
                                    Convert.ToString(reader["PhoneNumber"])!,
                                    Convert.ToString(reader["Country"])!,
                                    Convert.ToString(reader["Address"])!

                                );

                                guests.Add(guest);
                            }
                        }
                    }

                    return guests;

                }
                catch (Exception ex)
                {

                    throw new Exception("" + ex.Message);
                }

            }
        }

        public int GetApartment(int bookingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT ApartmentId FROM Bookings WHERE BookingId = @BookingId;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingId", bookingId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            return int.Parse(reader[0].ToString()!);
                        }
                    }

                }
                return 0;

            }
        }

        public Booking? GetBookingById(int bookingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT BookingId, CheckInDate, CheckOutDate, TotalPrice, 
                            Status FROM Bookings WHERE BookingId = @BookingId;";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingId", bookingId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BookingStatus Status = (BookingStatus)Enum.Parse(typeof(BookingStatus), reader["Status"].ToString()!);
                            Booking booking = new(

                                Convert.ToInt32(reader["BookingId"]),
                                Convert.ToDateTime(reader["CheckInDate"]),
                                Convert.ToDateTime(reader["CheckOutDate"]),
                                Convert.ToDecimal(reader["TotalPrice"]),
                                Status
                                );

                            return booking;
                        }
                    }

                }
                return null;
            }
        }

        public List<Booking> GetBookingsByUserId(int userId)
        {
            List<Booking> bookings = new List<Booking>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"
                    SELECT BookingId, ApartmentId, AccountHolderId, CheckInDate, CheckOutDate, TotalPrice, Status
                    FROM Bookings
                    WHERE AccountHolderId = @UserId;";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BookingStatus Status = (BookingStatus)Enum.Parse(typeof(BookingStatus), reader["Status"].ToString()!);

                            Booking booking = new(

                               Convert.ToInt32(reader["BookingId"]),
                               Convert.ToDateTime(reader["CheckInDate"]),
                               Convert.ToDateTime(reader["CheckOutDate"]),
                               Convert.ToDecimal(reader["TotalPrice"]),
                               Status
                            );

                            bookings.Add(booking);
                        }
                    }
                }
            }
            return bookings;
        }

        public void Update(Booking booking)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"UPDATE Bookings SET Status = @Status WHERE BookingId = @BookingId;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", booking.Status.ToString());
                    cmd.Parameters.AddWithValue("@BookingId", booking.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool IsOverlappingBookingExist(int apartmentId, DateTime checkInDate, DateTime checkOutDate)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"SELECT COUNT(*) FROM Bookings b
                                INNER JOIN Apartments a ON b.ApartmentId = a.ApartmentId
                                WHERE b.ApartmentId = @ApartmentId
                                AND b.CheckInDate < @CheckOutDate
                                AND b.CheckOutDate > @CheckInDate;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);
                    cmd.Parameters.AddWithValue("@CheckInDate", checkInDate);
                    cmd.Parameters.AddWithValue("@CheckOutDate", checkOutDate);
                    
                    using SqlDataReader reader = cmd.ExecuteReader();
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

        //public bool IsGuestAlreadyBooked(int guestProfileId, DateTime checkIn, DateTime checkOut)
        //{

        //    using (SqlConnection conn = new SqlConnection(_connectionString))
        //    {
        //        conn.Open();


        //        string query = @"SELECT COUNT(*) FROM BookingGuests bg
        //                     JOIN Bookings b ON bg.BookingId = b.BookingId
        //                     WHERE bg.GuestProfileId = @GuestProfileId
        //                     AND((b.CheckInDate < @CheckOut) AND(b.CheckOutDate > @CheckIn))";



        //        using (SqlCommand cmd = new SqlCommand(query, conn))
        //        {
        //            cmd.Parameters.AddWithValue("@GuestProfileId", guestProfileId);
        //            cmd.Parameters.AddWithValue("@CheckOut", checkOut);
        //            cmd.Parameters.AddWithValue("@CheckIn", checkIn);

        //            int count = (int)cmd.ExecuteScalar();//will return the first row based on the query
        //            return count > 0;
        //        }


        //    }

        //}

        //public List<Booking> GetBookingsForApartment(int apartmentId, DateTime? startDate = null, DateTime? endDate = null)
//        {
//            List<Booking> bookings = new List<Booking>();

//            using (SqlConnection conn = new SqlConnection(_connectionString))
//            {
//                conn.Open();

//                string query = @"
//            SELECT BookingId, CheckInDate, CheckOutDate, TotalPrice, Status, AccountHolderId 
//            FROM Bookings 
//            WHERE ApartmentId = @ApartmentId
//            AND Status != @CancelledStatus"; 

          
//                if (startDate.HasValue && endDate.HasValue)
//                {
//                    query += @" AND (
//                (CheckInDate < @EndDate) AND 
//                (CheckOutDate > @StartDate)
//            )";
//                }

//                using (SqlCommand cmd = new SqlCommand(query, conn))
//                {
//                    cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);
//                    cmd.Parameters.AddWithValue("@CancelledStatus", BookingStatus.Cancelled.ToString());

//                    if (startDate.HasValue && endDate.HasValue)
//                    {
//                        cmd.Parameters.AddWithValue("@StartDate", startDate.Value);
//                        cmd.Parameters.AddWithValue("@EndDate", endDate.Value);
//                    }

//                    using (SqlDataReader reader = cmd.ExecuteReader())
//                    {
//                        while (reader.Read())
//                        {
//                            Booking booking = new Booking(
//                                reader.GetDateTime(reader.GetOrdinal("CheckInDate")),
//                                reader.GetDateTime(reader.GetOrdinal("CheckOutDate"))
//                            )
//                            {
//                                Id = reader.GetInt32(reader.GetOrdinal("BookingId")),
//                                TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
//                                Status = Enum.Parse<BookingStatus>(reader.GetString(reader.GetOrdinal("Status")))
//                            };

//                            bookings.Add(booking);
//                        }
//                    }
//                }
//            }

//            return bookings;
//        }

//    }
//}
