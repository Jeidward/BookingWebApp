using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class DashboardIndexViewModel
    {
        public int TotalBookings { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public int UpcomingBookings { get; set; }
        public List<ActivityViewModel> Activities { get; set; }


        public static DashboardIndexViewModel ConvertToViewModel(DashboardAnalytics dashboardAnalytics)
        {
            return new DashboardIndexViewModel
            {
                TotalBookings = dashboardAnalytics.TotalBookings,
                TotalUsers = dashboardAnalytics.TotalUsers,
                TotalRevenue = dashboardAnalytics.TotalRevenue,
                UpcomingBookings = dashboardAnalytics.UpcomingBookings,
            };
        }




    }
}
