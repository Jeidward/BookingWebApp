using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class User
    {
        public int Id { get; }
        public string Name { get; }
        public string Email { get; }
        public string Password { get; private set; }
        public string Salt { get; private set; }

        public User(string email, string password,string name)
        {
            Email = email;
            Password = password;
            Name = name;
        }

        public User(int id,string email, string password, string name, string salt)
        {
            Id = id;
            Email = email;
            Password = password;
            Name = name;
            Salt = salt;
        }

        public User(int id, string email, string password, string name)
        {
            Id = id;
            Email = email;
            Password = password;
            Name = name;
        }

        public void SetPassword(string password)
        {
            this.Password = password;
        }
        public void SetSalt(string salt)
        {
            this.Salt = salt;
        }

    }
}
