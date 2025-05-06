using Enums;
using Models.Entities;
using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class BookingDto
    {
        public int Id { get; private set; } // possible will use the id for the refrence.
        public ApartmentDto Apartment { get; private set; }
        public List<GuestProfileDto> GuestProfiles { get; private set; }
        public DateTime CheckInDate { get; }
        public DateTime CheckOutDate { get; }
        public decimal TotalPrice { get; private set; }
        public BookingStatus Status { get; private set; }
        public ExtraService Service { get; private set; }

    }
}
