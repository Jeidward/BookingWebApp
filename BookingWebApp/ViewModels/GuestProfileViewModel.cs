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


        public static GuestProfileViewModel ConvertToGuestFromUser(User user)
        {
            return new GuestProfileViewModel
            {
                AccountId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Age = user.Age,
                Email =  user.Email,
                PhoneNumber = user.PhoneNumber,
                Country = user.Country,
                Adress = user.Address,
            };
        }

        public static GuestProfile ConvertToEntity(GuestProfileViewModel guestProfileViewModel)
        {
            return new GuestProfile(
                new AccountHolder(guestProfileViewModel.AccountId),
                guestProfileViewModel.FirstName,
                guestProfileViewModel.LastName,
                guestProfileViewModel.Age,
                guestProfileViewModel.Email,
                guestProfileViewModel.PhoneNumber,
                guestProfileViewModel.Country,
                guestProfileViewModel.Adress
            );
        }


    }
}
