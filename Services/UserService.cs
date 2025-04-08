using Models.Entities;
using Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;


namespace Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public bool Register(string email, string password, string name)
        {
            User newUser = new(email, password,name);
            //var hashedPW = HashPassword(password);

            if (string.IsNullOrWhiteSpace(newUser.Email) && string.IsNullOrEmpty(newUser.Password))
            {


                throw new ArgumentException("Email and Password cannot be empty.");
            }

            if (_userRepository.DoesUserExist(newUser.Email))
            {

                throw new ArgumentException("This email is already registered.");
            }
            
            newUser.SetPassword(HashPassword(password));
            return _userRepository.RegisterUser(newUser.Email,newUser.Password,newUser.Name);
        }

        public int GetExistedLogIn(string email, string password)
        {
            var hashPassword = HashPassword(password);
            return _userRepository.LogIn(email, hashPassword);
        }

        public User GetUser(int userId)
        {
            return _userRepository.GetUser(userId);    
        }


        public string HashPassword(string password)
        {
            SHA256 hash = SHA256.Create();
            var passwordBytes = Encoding.Default.GetBytes(password);
           var hashedPassword =  hash.ComputeHash(passwordBytes);
           return Convert.ToHexString(hashedPassword);
           
           
           // will aslo inlude salt, right now only hashing.
        }
    }
}
