using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.IRepositories;
using Interfaces.IServices;
using Models.Entities;

namespace Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewService(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        public int SaveReview(int apartmentId,Review review)
        {
            if (apartmentId <= 0)
                throw new ArgumentException("Apartment ID must be greater than zero.");
            this.ValidateReviewObject(review);

            return _reviewRepository.Save(apartmentId, review);
        }

        public List<Review>? GetReviewsForApartment(int apartmentId)
        {
            if (apartmentId <= 0)
                throw new ArgumentException("Apartment ID must be greater than zero.");
            return _reviewRepository.GetReviewsApartment(apartmentId);
        }

        public List<Review> GetAllReviews()
        {
            return _reviewRepository.GetAllReviews();
        }
        public decimal GetAverageRating(int apartmentId) // can use review
        {
            if (apartmentId <= 0)
                throw new ArgumentException("Apartment ID must be greater than zero.");
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

        private void ValidateReviewObject(Review review)
        {
            if(review.Account == null) throw new ArgumentException("Account cannot be null.");
            if (string.IsNullOrWhiteSpace(review.Comment)) throw new ArgumentException("Comment cannot be null or empty.");
            if (review.Rating == 0 || review.Rating > 5) throw new ArgumentException("Rating cannot be zero and greater than 5");
            if(review.CleanlinessRating == 0 || review.CleanlinessRating > 5) throw new ArgumentException("Cleanliness rating cannot be zero and greater than 5");
            if (review.ComfortRating == 0 || review.ComfortRating > 5) throw new ArgumentException("comfort rating cannot be zero and greater than 5");
            if (review.LocationRating == 0 || review.LocationRating > 5) throw new ArgumentException("Location rating cannot be zero and greater than 5");
            if (review.ValueRating == 0 || review.ValueRating > 5) throw new ArgumentException("Value rating cannot be zero and greater than 5");
        }

    }
}
