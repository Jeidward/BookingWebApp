using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entities
{
    public class DashboardAnalytics
    {
        public int TotalBookings { get;}

        public int TotalUsers { get; }

        public decimal TotalRevenue { get; }

        public int UpcomingBookings { get; }

        public DashboardAnalytics(int totalBookings, int totalUsers, decimal totalRevenue, int upcomingBookings)
        {
            TotalBookings = totalBookings;
            TotalUsers = totalUsers;
            TotalRevenue = totalRevenue;
            UpcomingBookings = upcomingBookings;
        }
    }
}
