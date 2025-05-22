using System.Runtime.CompilerServices;
using Models.Entities;
using static System.Net.Mime.MediaTypeNames;

namespace BookingWebApp.ViewModels
{
    public class ActivityViewModel
    {
        public string IconClass { get; set; } // e.g. "fas fa-shopping-cart text-primary"
        public string IconBgClass { get; set; } // e.g. "bg-primary bg-opacity-10"
        public string Title { get; set; } // e.g. "New order received"
        public string Description { get; set; } // e.g. "Order #123456 from John Doe"
        public string CustomerName { get; set; }
        public string TimeAgo { get; set; } // e.g. "Just now"
        public string BookingDate { get; set; } // e.g. "2023-10-01 12:00:00"

        public static ActivityViewModel ConvertToViewModelForNewBooking(ActivityDashboard activityDashboard)
        {
            return new ActivityViewModel
            {
                IconClass = "bi bi-house-fill text-primary",
                IconBgClass = "bg-primary bg-opacity-10",
                Title = "New booking received",
                Description = $"Booking #{activityDashboard.BookingId} from {activityDashboard.Name}",
                CustomerName = activityDashboard.Name,
                BookingDate = activityDashboard.BookingDate.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }

        public static ActivityViewModel ConvertToViewModelForBookingCancellation(ActivityDashboard activityDashboard)
        {
            return new ActivityViewModel
            {
                IconClass = "bi bi-house-fill text-danger",
                IconBgClass = "bg-danger bg-opacity-10",
                Title = "Booking cancelled",
                Description = $"Booking #{activityDashboard.BookingId} from {activityDashboard.Name} has been cancelled",
                CustomerName = activityDashboard.Name,
                TimeAgo = "Just now",
                BookingDate = activityDashboard.BookingDate.ToString("yyyy-MM-dd HH:mm:ss")
            };
        }
    }
}
