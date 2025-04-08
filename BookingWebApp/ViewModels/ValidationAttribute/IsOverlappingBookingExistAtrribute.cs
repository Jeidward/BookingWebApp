using System.ComponentModel.DataAnnotations;
using Services;

namespace BookingWebApp.ViewModels;

public class IsOverlappingBookingExistAttribute : ValidationAttribute
{
    private static BookingService _bookingService;
    
    public IsOverlappingBookingExistAttribute(BookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public IsOverlappingBookingExistAttribute()
    {
        
    }
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        _bookingService = (BookingService)validationContext.GetService(typeof(BookingService)); // need to research
        CreateBookingViewModel viewModel = (CreateBookingViewModel)validationContext.ObjectInstance;
     
        var existingBookings = _bookingService.IsOverlappingBookingExist(viewModel.ApartmentId,viewModel.CheckInDate,viewModel.CheckOutDate);
        if (existingBookings == true)
        {
            return new ValidationResult("An overlapping booking already exists.");
        }

        return ValidationResult.Success;
    }

       
}