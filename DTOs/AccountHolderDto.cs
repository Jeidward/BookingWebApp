using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class AccountHolderDto
    {
        public int Id { get; }
        public List<GuestProfileDto> Profiles { get; }
        public List<Booking> Bookings { get; set; }
    }
}
