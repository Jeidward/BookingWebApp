using Models.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Enums;
using System.Diagnostics.CodeAnalysis;
using Interfaces.IRepositories;


namespace MSSQL
{
    public class BookingRepository : Repository, IBookingRepository
    {
        private readonly AccountHolderRepository _accountHolderRepository;
        private readonly IApartmentRepository _apartmentRepository;
        public BookingRepository(IConfiguration configuration,IApartmentRepository apartmentRepository) : base(configuration)
        {
            _accountHolderRepository = new AccountHolderRepository(configuration);
            _apartmentRepository = apartmentRepository;
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
                     (ApartmentId, AccountHolderId, CheckInDate, CheckOutDate, TotalPrice, Status, IsArchived, CheckoutReminderSent,BookingCreated)
                     OUTPUT INSERTED.BookingId
                    VALUES (@ApartmentId, @AccountHolderId, @CheckInDate, @CheckOutDate, @TotalPrice, @Status,@IsArchived,@CheckoutReminderSent,@BookingCreated)";

                    using (SqlCommand bookingCmd = new SqlCommand(insertBookingSql, conn))
                    {
                        bookingCmd.Parameters.AddWithValue("@ApartmentId", booking.Apartment.Id); // booking already has apartmentId dont need to readh apartment
                        bookingCmd.Parameters.AddWithValue("@AccountHolderId", booking.GuestProfiles[0].Account.Id);
                        bookingCmd.Parameters.AddWithValue("@CheckInDate", booking.CheckInDate);
                        bookingCmd.Parameters.AddWithValue("@CheckOutDate", booking.CheckOutDate);
                        bookingCmd.Parameters.AddWithValue("@TotalPrice", booking.TotalPrice);
                        bookingCmd.Parameters.AddWithValue("@Status", booking.Status.ToString());
                        bookingCmd.Parameters.AddWithValue("@IsArchived", false);
                        bookingCmd.Parameters.AddWithValue("@CheckoutReminderSent", false);
                        bookingCmd.Parameters.AddWithValue("@BookingCreated", DateTime.Now);

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

                    //Insert into BookingGuests
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

        public Booking GetBookingById(int bookingId)
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
                        reader.Read();
                        {
                            BookingStatus Status = (BookingStatus)Enum.Parse(typeof(BookingStatus), reader["Status"].ToString()!);
                            return  new Booking( 

                                Convert.ToInt32(reader["BookingId"]),
                                Convert.ToDateTime(reader["CheckInDate"]),
                                Convert.ToDateTime(reader["CheckOutDate"]),
                                Convert.ToDecimal(reader["TotalPrice"]),
                                Status
                            );
                        }
                    }
                }
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
                    WHERE AccountHolderId = @UserId";

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
                               Convert.ToInt32(reader["AccountHolderId"]),
                                 Convert.ToInt32(reader["ApartmentId"]),
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

        public void CancelBooking(int bookingId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"UPDATE Bookings SET Status = @Status WHERE BookingId = @BookingId;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", BookingStatus.Cancelled.ToString());
                    cmd.Parameters.AddWithValue("@BookingId", bookingId);
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
                                AND b.Status = 'Confirmed'  
                                AND b.CheckInDate <= @CheckOutDate
                                AND b.CheckOutDate >= @CheckInDate;";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@ApartmentId", apartmentId);
                    cmd.Parameters.AddWithValue("@CheckInDate", checkInDate);
                    cmd.Parameters.AddWithValue("@CheckOutDate", checkOutDate);
                    int count = (int)cmd.ExecuteScalar(); 
                    return count > 0; 
                }
                
            }
        }

        public List<(Booking,string, string)> GetBookingsDue(DateTime date)
        {
            const string sql = @"
            SELECT  b.BookingId,
                    b.ApartmentId,
                    b.CheckInDate,
                    b.CheckOutDate,
                    b.TotalPrice,
                    b.Status,
                    b.CheckoutReminderSent,
                    u.Email,
                    u.FirstName
            FROM    Bookings AS b
            JOIN    AccountHolders ah ON ah.AccountHolderId = b.AccountHolderId
            JOIN    Users          u  ON u.UserId          = ah.UserId
            WHERE   CAST(b.CheckOutDate AS date)      = @Today
            AND   b.CheckoutReminderSent = 0;";

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Today", date.Date);

            conn.Open();
            using var reader = cmd.ExecuteReader();

            var results = new List<(Booking, string, string)>();

            while (reader.Read())
            {
                BookingStatus status = (BookingStatus)Enum.Parse(typeof(BookingStatus), reader["Status"].ToString()!);
                var booking = new Booking(
                    Convert.ToInt32(reader["bookingId"]),
                       Convert.ToDateTime(reader["CheckInDate"]),
                       Convert.ToDateTime(reader["CheckOutDate"]), 
              Convert.ToDecimal(reader["TotalPrice"]),
                       status
                );

                booking.SetCheckoutReminderSent(Convert.ToBoolean(reader["CheckoutReminderSent"]));
                booking.SetApartment(_apartmentRepository.GetApartment(Convert.ToInt32(reader["ApartmentId"])));
                string email = Convert.ToString(reader["Email"])!;
                var name = Convert.ToString(reader["FirstName"])!;

                results.Add((booking, email,name));         
            }
            return results;                             
        }

        public int GetAllBookings(int selectedMonth, int year)
        {
            List<Booking> bookings = new List<Booking>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"SELECT BookingId, ApartmentId, AccountHolderId, CheckInDate, CheckOutDate, TotalPrice, Status, BookingCreated
                            FROM Bookings WHERE Status = 'Confirmed' AND MONTH(BookingCreated) = @BookingCreated  AND YEAR(BookingCreated) = @Year";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@BookingCreated", selectedMonth);
                    cmd.Parameters.AddWithValue("@Year", year);
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

            return bookings.Count;
        }

        public List<Booking> GetAllBookingsWithObject()
        {
            List<Booking> bookings = new List<Booking>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string sql = @"SELECT BookingId, ApartmentId, AccountHolderId, CheckInDate, CheckOutDate, TotalPrice, Status
                            FROM Bookings WHERE Status = 'Confirmed'";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            BookingStatus Status = (BookingStatus)Enum.Parse(typeof(BookingStatus), reader["Status"].ToString()!);
                            Booking booking = new(
                                Convert.ToInt32(reader["BookingId"]),
                                Convert.ToInt32(reader["ApartmentId"]),
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
        public void MarkCheckoutReminderSent(int bookingId)
        {
            const string sql =
                @"UPDATE Bookings
                SET    CheckoutReminderSent = 1
                WHERE  BookingId = @Id;";

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@Id", bookingId);

            conn.Open();
            cmd.ExecuteNonQuery();
        }

        public List<ActivityDashboard> GetAllActivitiesObjects()
        {
            var activities = new List<ActivityDashboard>();
            using SqlConnection conn = new SqlConnection(_connectionString);
            conn.Open();
            string sql = @"SELECT  b.[BookingId]
	                               ,a.UserId
	                               ,u.FirstName
	                               ,b.BookingCreated
                                   ,b.Status
                                    FROM [dbo].[Bookings] as b
                                    INNER JOIN AccountHolders	as a on a.AccountHolderId = b.AccountHolderId
                                    INNER JOIN [Users] as u on u.UserId = a.UserId 
                                    ORDER BY b.BookingCreated Desc";
            
            using SqlCommand cmd = new SqlCommand(sql, conn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var bookingId = Convert.ToInt32(reader["BookingId"]);
                var userId = Convert.ToInt32(reader["UserId"]);
                var name = reader["FirstName"].ToString()!;
                var bookingCreated = Convert.ToDateTime(reader["BookingCreated"]);
                var status = (BookingStatus)Enum.Parse(
                    typeof(BookingStatus),
                    reader["Status"].ToString()!
                );

                activities.Add(new ActivityDashboard(
                    bookingId,
                    userId,
                    name,
                    bookingCreated,
                    status
                ));

            }
            return activities;
        }

        public async Task<PaginatedList<ActivityDashboard>> GetAllActivitiesObjectAsync(int pageIndex,int pageSize)
        {
            await using SqlConnection conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();
            string query = $@"SELECT b.[BookingId]
	                               ,a.UserId
	                               ,u.FirstName
	                               ,b.BookingCreated
                                   ,b.Status
                                    FROM [dbo].[Bookings] as b
                                    INNER JOIN AccountHolders	as a on a.AccountHolderId = b.AccountHolderId
                                    INNER JOIN [Users] as u on u.UserId = a.UserId 
                                    ORDER BY b.BookingCreated Desc, b.BookingId OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var total = (int)(await new SqlCommand("SELECT COUNT(*) FROM [dbo].[Bookings]",conn).ExecuteScalarAsync());
            await using SqlCommand cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Offset", (pageIndex - 1) * pageSize);
            cmd.Parameters.AddWithValue("@PageSize", pageSize);
            await using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            var activities = new List<ActivityDashboard>();
            while (await reader.ReadAsync())
            {
                var bookingId = Convert.ToInt32(reader["BookingId"]);
                var userId = Convert.ToInt32(reader["UserId"]);
                var name = reader["FirstName"].ToString()!;
                var bookingCreated = Convert.ToDateTime(reader["BookingCreated"]);
                var status = (BookingStatus)Enum.Parse(
                    typeof(BookingStatus),
                    reader["Status"].ToString()!
                );
                activities.Add(new ActivityDashboard(
                    bookingId,
                    userId,
                    name,
                    bookingCreated,
                    status
                ));
            }
            return PaginatedList<ActivityDashboard>.Create(activities,total,pageIndex, pageSize);
        }
    }
}
    