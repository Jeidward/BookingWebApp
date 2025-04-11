using System.ComponentModel.DataAnnotations;
using BookingWebApp.ViewModels;
using Services;

namespace BookingWebApp.ViewModel
{
    public class EmailCheckAttribute : ValidationAttribute
    {
        private UserService _userService;
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var viewModel = (UserViewModel)validationContext.ObjectInstance;
            _userService = (UserService)validationContext.GetService(typeof(UserService));

            var emailAlreadyExist = _userService.DoesUserExist(viewModel.Email);

            if (emailAlreadyExist)
            {
                return new ValidationResult("Email already exists.");
            }

            return ValidationResult.Success;
        }
    }
}
