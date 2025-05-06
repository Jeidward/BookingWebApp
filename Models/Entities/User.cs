using System.ComponentModel.DataAnnotations;

namespace Models.Entities
{
    public class User
    {
        public int Id { get; }
        public string Name { get; }
        public string Email { get; private set; } // can remove this latter as set is use for testing.
        public string Password { get; private set; }
        public string Salt { get; private set; }
        public int RoleId { get; private set; }

        public User(string email, string password,string name)
        {
            Email = email;
            Password = password;
            Name = name;
        }
        public User(int id)
        {
            Id = id;
        }

        public User(int id,string email, string password, string name, string salt, int roleId)
        {
            Id = id;
            Email = email;
            Password = password;
            Name = name;
            Salt = salt;
            RoleId = roleId;
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
