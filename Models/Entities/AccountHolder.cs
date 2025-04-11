using System.Text.Json.Serialization;

namespace Models.Entities
{
    public class AccountHolder
    {
        public int Id { get;  }
        public List<GuestProfile> Profiles { get; }
        public List<Booking> Bookings { get; set; }

        public AccountHolder(int id)
        {
            Id = id;
            Profiles = new List<GuestProfile>();
            Bookings = new List<Booking>();
        }

        public static AccountHolder DefaultAccountHolder() //this may be all of my entities.
        {
            return new AccountHolder(-1);
        }
    }
}
