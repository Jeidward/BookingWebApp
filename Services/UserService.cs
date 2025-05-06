using Models.Entities;
using Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;


namespace Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordSecurityService _passwordSecurityService;
        
        public UserService(IUserRepository userRepository, PasswordSecurityService passwordSecurityService)
        {
            _userRepository = userRepository;
            _passwordSecurityService = passwordSecurityService;
        }
        public bool Register(string email, string password, string name)
        {
            User newUser = new(email, password,name);
            
            var hashedPassword = _passwordSecurityService.HashPassword(password, out byte[] salt);
            newUser.SetPassword(hashedPassword);
            newUser.SetSalt(Convert.ToBase64String(salt));
            return _userRepository.RegisterUser(newUser.Email,newUser.Password,newUser.Name, newUser.Salt);
        }

        public int GetExistedLogIn(string email, string password)
        {
            var user = _userRepository.GetUser(email);
            if (user.Id == -1)
            {
                return 0;
            }

            var storedSalt = Convert.FromBase64String(user.Salt);
            var hashPassword = _passwordSecurityService.HashPassword(password, storedSalt);

            return _userRepository.LogIn(email, hashPassword);
        }

        public User GetUser(int userId)
        {
            return _userRepository.GetUser(userId);    
        }

        public User GetUserWithEmail(string email)
        {
            return _userRepository.GetUser(email);
        }

        public bool DoesUserExist(string email)
        {
            return _userRepository.DoesUserExist(email);
        }

    }
}
