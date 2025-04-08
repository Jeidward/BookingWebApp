using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces;
using Models.Entities;

namespace Services
{
    public class ReviewService
    {
        
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public int SaveReview(int apartmentId,Review review)
        {
            return _reviewRepository.Save(apartmentId, review);
        }

        public List<Review> GetReviewsForApartment(int apartmentId)
        {
            return _reviewRepository.GetReviewsApartment(apartmentId);
        }
        
    }
}
