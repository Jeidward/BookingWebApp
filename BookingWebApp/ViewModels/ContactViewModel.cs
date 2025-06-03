using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class ContactViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastMessage { get; set; }
        public int ApartmentId { get; set; }


        public static ContactViewModel ConvertToViewModel(User user, Booking booking)
        {
            return new ContactViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                ApartmentId = booking?.ApartmentId ?? 0
            };
        }

        public static List<ContactViewModel> ConvertToViewModel(List<User> users, List<Booking> bookings)
        {
            var contacts = new List<ContactViewModel>();
            foreach (var user in users)
            {
                var booking = bookings.FirstOrDefault(b => b.AccountHolderId == user.Id);
                contacts.Add(ConvertToViewModel(user, booking));
            }
            return contacts;
        }

    }
}
