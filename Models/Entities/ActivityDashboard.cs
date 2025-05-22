using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Enums;

namespace Models.Entities
{
    public class ActivityDashboard
    {
        public int BookingId { get;}
        public int UserId { get; }
        public string Name { get;}
        public DateTime BookingDate { get; }

        public BookingStatus Status { get; set; }

        public ActivityDashboard(int bookingId, int userId,string name,DateTime bookingDate ,BookingStatus status)
        {
            BookingId = bookingId;
            UserId = userId;
            Name = name;
            BookingDate = bookingDate;
            Status = status;
        }
    }
}
