using Microsoft.Data.SqlClient;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces
{
    public interface IApartmentRepository
    {
        public List<Apartment> GetApartments(int count);

        public Apartment GetApartment(int id);

        public List<string> GetGallery(int id);


    }
}
    

