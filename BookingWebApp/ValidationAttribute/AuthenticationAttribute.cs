using Services;
using System.ComponentModel.DataAnnotations;

namespace BookingWebApp.ViewModels
{
    public class AuthenticationAttribute : ValidationAttribute
    {
        private UserService _userService;
        
        
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var viewModel = (NonDetailUserViewModel)validationContext.ObjectInstance;
            _userService = (UserService)validationContext.GetService(typeof(UserService));

            var userId = _userService.GetExistedLogIn(viewModel.Email, viewModel.Password);
            if (userId == 0)
            {
                return new ValidationResult("Invalid email or password.");
            }
            
            return ValidationResult.Success;
        }


    }
}
