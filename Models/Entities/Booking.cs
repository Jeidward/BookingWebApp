using Enums;
using Models.Enums;


namespace Models.Entities
{
    public class Booking
    {
        public int Id { get; private set; } 

        public int ApartmentId { get; private set; } // hmmm not sure if i need to let booking know about the apartment
        public Apartment Apartment { get; private set; }
        public List<GuestProfile> GuestProfiles { get; private set; }
        public DateTime CheckInDate { get; }
        public DateTime CheckOutDate { get; }
        public decimal TotalPrice { get; private set; }
        public BookingStatus Status { get; private set; }
        public ExtraService Service { get; }
        public bool CheckoutReminderSent { get; private set; }



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

        public Booking(int id,int apartmentId ,DateTime checkInDate, DateTime checkOutDate, decimal totalPrice, BookingStatus status)
        {
            Id = id;
            ApartmentId = apartmentId;
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
        public Booking(int id,DateTime checkIn, DateTime checkOut, decimal totalPrice, Apartment apartment, List<GuestProfile> guestProfile)
        {
            Id = id;
            GuestProfiles = guestProfile;
            CheckInDate = checkIn;
            CheckOutDate = checkOut;
            TotalPrice = totalPrice;
            Apartment = apartment;
        }

        public Booking(int id)
        {
            Id = id;
        }

        // empty constructor dont forget.

        public void SetCheckoutReminderSent(bool checkoutReminderSent)
        {
            CheckoutReminderSent = checkoutReminderSent;
        }
        
        public void SetGuestProfile(List<GuestProfile> guestProfile)
        {
            GuestProfiles = guestProfile;
        }

        public void SetApartment(Apartment apartment)
        {
            Apartment = apartment;
        }

        public void SetApartments(List<Apartment> apartments)
        {
            foreach (var apartment in apartments)
            {
                Apartment = apartment;
            }

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
