using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.IRepositories
{
    public interface IAmenitiesRepository
    { 
        public List<Amenities> GetAmenities(int id);
        public List<Amenities> GetAmenitiesList();
        public void AddAmenities(int apartmentId, int amenityId);
        public List<Amenities> GetSelectedAmenities(int apartmentId);
    }
}
