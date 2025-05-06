using Models.Entities;
using BookingWebApp.ViewModels;
using Services;

namespace BookingWebApp.Helpers
{
    public class GuestProfileViewModelHelper
    {
        private readonly AccountHolderService _accountHolderService;
        public GuestProfileViewModelHelper(AccountHolderService accountHolderService)
        {
            _accountHolderService = accountHolderService;   
        }

        public static string CreateGuestProfileString(GuestProfileViewModel guest)
        {
            return $"AccountHolderId:{guest.AccountId}$FirstName:{guest.FirstName}$LastName:{guest.LastName}$Age:{guest.Age}$Email:{guest.Email}$PhoneNumber:{guest.PhoneNumber}$Country:{guest.Country}$Adress:{guest.Adress}";
        }

        public GuestProfile ReadGuestProfileString(string guestString)
        {
            string[] guestData = guestString.Split('$');
            // the first string is our key and the second string is our string data, for example guest.firstname
            Dictionary<string, string> dataPairs = new Dictionary<string, string>();
            foreach (string data in guestData)
            {
                dataPairs.Add(data.Split(':')[0], data.Split(':')[1]);
            }

            AccountHolder account = _accountHolderService.GetAccountHolderById(int.Parse(dataPairs["AccountHolderId"]));
            return new(account, dataPairs["FirstName"], dataPairs["LastName"], int.Parse(dataPairs["Age"]), dataPairs["Email"], dataPairs["PhoneNumber"], dataPairs["Country"], dataPairs["Adress"]);
        }
    }
}
