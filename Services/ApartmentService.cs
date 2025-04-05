using Microsoft.Extensions.Configuration;
using Interfaces;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ApartmentService
    {
        //private readonly ApartmentRepository _apartmentRepository;
        private readonly IApartmentRepository _IapartmentRepository;

        public ApartmentService(IApartmentRepository apartmentRepository)
        {
            _IapartmentRepository = apartmentRepository;
        }

        public Apartment GetApartment(int id)
        {
            Apartment selectedApartment = _IapartmentRepository.GetApartment(id);

            selectedApartment.SetGallery(_IapartmentRepository.GetGallery(id));
            return selectedApartment;
        }

        public List<Apartment> GetAllApartments(int count)
        {
            return _IapartmentRepository.GetApartments(count);

        }

    }
}
