using System.Text.Json.Serialization;

namespace Models.Entities
{
    public class AccountHolder
    {
        public int Id { get; set; }
        public List<GuestProfile> Profiles { get; set; }
        public List<Booking> Bookings { get; set; }

        public AccountHolder(int id)
        {
            Id = id;
            Profiles = new List<GuestProfile>();
            Bookings = new List<Booking>();
        }

    }
}
