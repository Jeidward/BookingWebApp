namespace BookingWebApp.ViewModels
{
    public class DashboardIndexViewModel
    {
        public int TotalBookings { get; set; }
        public int TotalUsers { get; set; }
        
        public static DashboardIndexViewModel ConvertToViewModel(int totalBookings)
        {
            return new DashboardIndexViewModel
            {
                TotalBookings = totalBookings,
            };
        }
    }
}
