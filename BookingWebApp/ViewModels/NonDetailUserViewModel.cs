using System.ComponentModel.DataAnnotations;


namespace BookingWebApp.ViewModels
{
    public class NonDetailUserViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        [Display(Name = "Password")]
        public string Password { get; set; }
    }
}
