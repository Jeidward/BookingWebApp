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
            if(reviews==null)
            {
                reviews = new List<Review>();
            }
            selectedApartment.SetReviews(reviews);
            selectedApartment.SetAvgRating(_reviewService.GetAverageRating(id));
            selectedApartment.SetReviewsCount(reviews.Count);
            selectedApartment.SetGallery(_apartmentRepository.GetGallery(id));
            return selectedApartment;
        }

        public List<Apartment> GetAllApartments(int count)
        {
            var apartments = _apartmentRepository.GetApartments(count);
            foreach (var apartment in apartments)
            {
                var reviews = _reviewService.GetReviewsForApartment(apartment.Id);
                if (reviews == null)
                {
                    reviews = new List<Review>();
                }
                apartment.SetReviews(reviews);
                apartment.SetAvgRating(_reviewService.GetAverageRating(apartment.Id));
                apartment.SetReviewsCount(reviews.Count);
                apartment.SetGallery(_apartmentRepository.GetGallery(apartment.Id));
            }
            return apartments;
        }

    }
}
