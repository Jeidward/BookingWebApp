using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class GuestProfileViewModel
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Country { get; set; }
        public string Adress { get; set; }


        public static GuestProfileViewModel ConvertToViewModel(GuestProfile guestProfile)
        {
            return new GuestProfileViewModel
            {
                AccountId = guestProfile.Account.Id,
                FirstName = guestProfile.FirstName,
                LastName = guestProfile.LastName,
                Age = guestProfile.Age,
                Email = guestProfile.Email,
                PhoneNumber = guestProfile.PhoneNumber,
                Country = guestProfile.Country,
                Adress = guestProfile.Address,

            };
        }


    }
}
