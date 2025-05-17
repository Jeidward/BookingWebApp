using Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.IServices
{
    public interface IReviewService
    {
        public int SaveReview(int apartmentId, Review review);

        public List<Review>? GetReviewsForApartment(int apartmentId);

        public List<Review> GetAllReviews();

        public decimal GetAverageRating(int apartmentId);

    }
}
