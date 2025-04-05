using System.ComponentModel.DataAnnotations;
namespace BookingWebApp.ViewModels
{
    public class DateRangeAttribute: ValidationAttribute
    { 
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            CreateBookingViewModel viewModel = (CreateBookingViewModel)validationContext.ObjectInstance;
            
            if (viewModel.CheckOutDate < viewModel.CheckInDate)
            {
                return new ValidationResult("Check-out date must be after check-in date.");
            }

            return ValidationResult.Success;
        }
    }
}
