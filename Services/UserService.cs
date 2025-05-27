using Models.Entities;
using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Interfaces.IServices;
using Interfaces.IRepositories;


namespace Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordSecurityService _passwordSecurityService;
        
        public UserService(IUserRepository userRepository, IPasswordSecurityService passwordSecurityService)
        {
            _userRepository = userRepository;
            _passwordSecurityService = passwordSecurityService;
        }

            
        public DomainValidationResult Register(User user) // this could just to test if it run it one time
        {
            var result = UserValidator.ValidUser(user.Email, user.Password, user.FirstName);
            if (result.IsValid)
            {
                var hashedPassword = _passwordSecurityService.HashPassword(user.Password, out byte[] salt);
                user.SetPassword(hashedPassword);
                user.SetSalt(Convert.ToBase64String(salt));

                 _userRepository.RegisterUser(user);
            }

            return result;
        }

        public List<Claim> CreateClaims(int userId, string email)
        {
            var user =  GetUserWithEmail(email);

            var role = user.RoleId == 2 ? "Host" : "User";

            var claims = new List<Claim>
            {
                new("Id", userId.ToString()),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Role, role)
            };
            return claims;
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
