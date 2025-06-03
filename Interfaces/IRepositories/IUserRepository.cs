using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.IRepositories
{
    public interface IUserRepository
    {
        public bool RegisterUser(User user);

        public bool DoesUserExist(string email);

        public int LogIn(string email, string password);

        public User GetUser(int id);

        public User GetUser(string email);

        public List<User> GetAllUsers();

    }
}
