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
        private readonly IApartmentRepository _apartmentRepository;
        private readonly ReviewService _reviewService;

        public ApartmentService(IApartmentRepository apartmentRepository, ReviewService reviewService)
        {
            _apartmentRepository = apartmentRepository;
            _reviewService = reviewService;
        }

        public Apartment GetApartment(int id)
        {
            Apartment selectedApartment = _apartmentRepository.GetApartment(id);
            var reviews = _reviewService.GetReviewsForApartment(id);
            selectedApartment.SetReviews(reviews);
            selectedApartment.SetGallery(_apartmentRepository.GetGallery(id));
            return selectedApartment;
        }

        public List<Apartment> GetAllApartments(int count)
        {
            return _apartmentRepository.GetApartments(count);

        }

    }
}
