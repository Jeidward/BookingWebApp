using Enums;


namespace Models.Entities
{
    public class Booking
    {
        public int Id { get; private set; } // possible will use the id for the refrence.
        public Apartment Apartment { get; private set; }
        public List<GuestProfile> GuestProfiles { get; private set; }
        public DateTime CheckInDate { get; }
        public DateTime CheckOutDate { get; }
        public decimal TotalPrice { get; }
        public BookingStatus Status { get; private set; }

        /// <summary>
        /// this is for the repo
        /// </summary>
        /// <param name="checkIn"></param>
        public Booking(int id,DateTime checkInDate, DateTime checkOutDate,decimal totalPrice, BookingStatus status)
        {
            Id = id;
            CheckInDate = checkInDate;
            CheckOutDate = checkOutDate;
            TotalPrice = totalPrice;
            Status = status;
        }
        public Booking(DateTime checkIn, DateTime checkOut, decimal totalPrice, Apartment apartment)//dont need guestprofile
        {
            GuestProfiles = new List<GuestProfile>();
            CheckInDate = checkIn;
            CheckOutDate = checkOut;
            TotalPrice = totalPrice;
            Apartment = apartment;  
        }

        //use for test
        public Booking(DateTime checkIn, DateTime checkOut, decimal totalPrice, Apartment apartment, List<GuestProfile> guestProfile)//dont need guestprofile
        {
            GuestProfiles = guestProfile;
            CheckInDate = checkIn;
            CheckOutDate = checkOut;
            TotalPrice = totalPrice;
            Apartment = apartment;
        }

        
        public void SetGuestProfile(List<GuestProfile> guestProfile)
        {
            GuestProfiles = guestProfile;
        }

        public void SetApartment(Apartment apartment)
        {
            Apartment = apartment;
        }
        public void SetId(int id)
        {
            Id = id;
        }

        public void SetStatus(BookingStatus status)
        {
            this.Status = status;   
        }


    }
}
