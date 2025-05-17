namespace Models.Entities
{
    public class GuestProfile
    {
        public int Id { get; }
        public AccountHolder Account { get; private set; }
        public string FirstName { get; }
        public string LastName { get; }
        public int Age { get; }
        public string Email { get; }
        public string PhoneNumber { get; }
        public string Country { get; }
        public string Address { get; }

        public GuestProfile(AccountHolder account, string firstName, string lastName, int age, string email, string phoneNumber, string country, string address)
        {
            Account = account;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            Email = email;
            PhoneNumber = phoneNumber;
            Country = country;
            Address = address;

        }

        public GuestProfile(GuestProfile guest)
        {
            Account = guest.Account;
            FirstName = guest.FirstName;
            LastName = guest.LastName;
            Age = guest.Age;
            Email = guest.Email;
            PhoneNumber = guest.PhoneNumber;
            Country = guest.Country;
            Address = guest.Address;
        }

        public void SetAccountHolder(AccountHolder accountHolder)
        {
            Account = accountHolder;
        }



    }
}
