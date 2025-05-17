using Models.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Metrics;
using Interfaces.IRepositories;


namespace Services
{
    public class AccountHolderService
    {
        private readonly IAccountHolderRepository _accountHolderRepository;

        public AccountHolderService(IAccountHolderRepository accountHolderRepository)   
        {
            _accountHolderRepository = accountHolderRepository; 
        }

        public GuestProfile CreateGuestProfile(GuestProfile guest)
        {
            this.ValidateForCreateGuestProfile(guest);
            return new(guest);
        }

        public AccountHolder? GetAccountHolderById(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be greater than 0"); // this needed even??
            return _accountHolderRepository.GetAccountHolder(id);
        }

        public AccountHolder GetAccountHolderByUserId(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be greater than 0");
            return _accountHolderRepository.GetAccountHolderByUserId(id);
        }

        public bool HasAccountHolderAnyBooking(int id)
        {
            if (id <= 0) throw new ArgumentException("Id must be greater than 0");
            return _accountHolderRepository.HasBookingForAccountHolder(id);
        }


        private void ValidateForCreateGuestProfile(GuestProfile guest)
        {
            if(guest.Account.Id == 0) throw new ArgumentException("AccountHolder is required");
            if (string.IsNullOrWhiteSpace(guest.FirstName)) throw new ArgumentException("firstname is required.");
            if (string.IsNullOrWhiteSpace(guest.LastName)) throw new ArgumentException("Lastname is required");
            if (string.IsNullOrWhiteSpace(guest.Email)) throw new ArgumentException("email is required");
            if (string.IsNullOrWhiteSpace(guest.PhoneNumber)) throw new ArgumentException("phone number is required");
            if (string.IsNullOrWhiteSpace(guest.Country)) throw new ArgumentException("creditcard number is required");
            if (string.IsNullOrWhiteSpace(guest.Address)) throw new ArgumentException("address should not be empty");
        }


    }
}
