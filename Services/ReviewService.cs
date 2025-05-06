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

        public List<Review>? GetReviewsForApartment(int apartmentId)
        {
            return _reviewRepository.GetReviewsApartment(apartmentId);
            // possible get also the avarage rating 
        }

        //public List<Review> GetReviewsForAllApartments()
        //{
        //    return _reviewRepository.GetReviewsForAllApartments();
        //}

        public List<Review> GetAllReviews()
        {
            return _reviewRepository.GetAllReviews();
        }
        public decimal GetAverageRating(int apartmentId) // can use review
        {
            var reviews = _reviewRepository.GetReviewsApartment(apartmentId);
            if (reviews == null || reviews.Count == 0)
            {
                return 0;
            }
            decimal totalRating = 0;
            foreach (var review in reviews)
            {
                totalRating += review.Rating;
            }
            return totalRating / reviews.Count;
        }

    }
}
