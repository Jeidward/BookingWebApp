using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTOs
{
    public class GuestProfileDto
    {
        public int Id { get; }
        public AccountHolderDto Account { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public int Age { get; }
        public string Email { get; }
        public string PhoneNumber { get; }
        public string Country { get; }
        public string Address { get; }
    }
}
