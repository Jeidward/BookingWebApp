using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IPasswordSecurityService
    {
        public string HashPassword(string password, out byte[] salt);

        public string HashPassword(string password, byte[] salt);

    }
}
