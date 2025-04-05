using System.ComponentModel.DataAnnotations;
using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string  Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public UserViewModel()
        {
        }

        public UserViewModel(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }
        public static UserViewModel ConvertToViewModel(User user)
        {
            return new UserViewModel(name: user.Name, email: user.Email, password: user.Password);
        }



    }
}
