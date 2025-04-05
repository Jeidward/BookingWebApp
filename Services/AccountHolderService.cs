using Models.Entities;
using Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services
{
    public class AccountHolderService
    {
        private readonly IAccountHolderRepository _accountHolderRepository;

        public AccountHolderService(IAccountHolderRepository accountHolderRepository)   
        {
            _accountHolderRepository = accountHolderRepository; 
        }

        public GuestProfile CreateGuestProfile(AccountHolder accountHolder, string firstName, string lastName, int age, string email, string phoneNumber, string country, string adress)
        {
            if (string.IsNullOrWhiteSpace(firstName)) throw new ArgumentException("firtname is required.");
            if (string.IsNullOrWhiteSpace(lastName)) throw new ArgumentException("Lastname is required");
            if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("email is required");
            if (string.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentException("phone number is required");
            if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("creditcard number is required");
            //the rest

            return new(accountHolder, firstName, lastName, age, email, phoneNumber, country, adress);
        }

       

        public AccountHolder? GetAccountHolderById(int id)
        {
            return _accountHolderRepository.GetAccountHolder(id);
        }

        public AccountHolder GetAccountHolderByUserId(int id)
        {
            return _accountHolderRepository.GetAccountHolderByUserId(id);
        }

        public bool HasAccountHolderAnyBooking(int id)
        {
            return _accountHolderRepository.HasBookingForAccountHolder(id);
        }


    }
}
