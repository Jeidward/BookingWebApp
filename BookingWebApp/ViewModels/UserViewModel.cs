using System.ComponentModel.DataAnnotations;
using BookingWebApp.ViewModel;
using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Age is required")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public UserViewModel()
        {
        }

        public UserViewModel(string firstName, string email, string password)
        {
            FirstName = firstName;
            Email = email;
            Password = password;
        }

        public static UserViewModel ConvertToViewModel(User user)
        {
            return new UserViewModel(firstName: user.FirstName, email: user.Email, password: user.Password);
        }

        public static User ConvertToEntity(UserViewModel userViewModel)
        {
            return new User(
                firstName: userViewModel.FirstName,
                lastName: userViewModel.LastName,
                age: userViewModel.Age,
                phoneNumber: userViewModel.PhoneNumber,
                country: userViewModel.Country,
                address: userViewModel.Address,
                email: userViewModel.Email,
                password: userViewModel.Password
            );





        }
    }
}
