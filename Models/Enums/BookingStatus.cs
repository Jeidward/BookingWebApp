using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Enums
{
    public enum BookingStatus
    {
        Pending,    // Booking created but not confirmed or paid
        Confirmed,  // Payment received or otherwise confirmed
        CheckedIn,  // Guest has arrived
        CheckedOut, // Guest has left
        Cancelled,  // Booking was cancelled
        Completed   // Stays that concluded successfully 
    }
}
