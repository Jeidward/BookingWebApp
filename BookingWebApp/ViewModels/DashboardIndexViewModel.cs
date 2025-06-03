using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class DashboardIndexViewModel
    {
        public int TotalBookings { get; set; }
        public int TotalUsers { get; set; }
        public decimal TotalRevenue { get; set; }
        public bool IsPreviousMonth { get; set; }
        public DateTime CurrentDate { get; set; }
        public int UpcomingBookings { get; set; }
        public PaginatedList<ActivityViewModel> Activities { get; set; }



        public static DashboardIndexViewModel ConvertToViewModel(DashboardAnalytics dashboardAnalytics, PaginatedList<ActivityViewModel> activities, DateTime currentDate, bool isPreviousMonth)
        {
            return new DashboardIndexViewModel
            {
                TotalBookings = dashboardAnalytics.TotalBookings,
                TotalUsers = dashboardAnalytics.TotalUsers,
                TotalRevenue = dashboardAnalytics.TotalRevenue,
                UpcomingBookings = dashboardAnalytics.UpcomingBookings,
                Activities = activities,
                IsPreviousMonth = isPreviousMonth,
                CurrentDate = currentDate
            };
        }

      




    }
}
