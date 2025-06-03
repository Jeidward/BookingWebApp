using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class User
    {
        public int Id { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get;} 
        public int Age { get; }
        public string PhoneNumber { get; }
        public string Country { get; }
        public string Address { get; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public string Salt { get; private set; }
        public int RoleId { get; private set; }

        public User(string firstName,string lastName,int age,string phoneNumber,string country,string address,string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            PhoneNumber = phoneNumber;
            Country = country;
            Address = address;
            Email = email;
            Password = password;
            FirstName = firstName;
        }
        public User(int id)// for test
        {
            Id = id;
        }

        public User(int id,string email, string password, string firstName, string salt, int roleId)// for test
        {
            Id = id;
            Email = email;
            Password = password;
            FirstName = firstName;
            Salt = salt;
            RoleId = roleId;
        }

        public User(int id,int roleId ,string firstName, string lastName, int age, string phoneNumber, string country, string address, string email) // for repo
        {
            Id = id;
            RoleId = roleId;
            FirstName = firstName;
            LastName = lastName;
            Age = age;
            PhoneNumber = phoneNumber;
            Country = country;
            Address = address;
            Email = email;
        }

        public void SetFirstName(string firstName)
        {
            this.FirstName = firstName;
        }
        public void SetPassword(string password)
        {
            this.Password = password;
        }
        public void SetSalt(string salt)
        {
            this.Salt = salt;
        }

        public void SetEmail(string email)
        {
            this.Email = email;
        }

        public void SetRoleId(int id)
        {
            this.RoleId = id;
        }

        
    }
}
