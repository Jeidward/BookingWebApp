namespace Models.Entities
{
    public class GuestProfile
    {
        public int Id { get; }
        public AccountHolder Account { get; }
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

    }
}
