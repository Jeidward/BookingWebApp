using Models.Entities;

namespace Interfaces.IRepositories
{
    public interface IReviewRepository
    {
        public int Save(int apartmentId, Review review);
        public List<Review>? GetReviewsApartment(int apartmentId);
        public List<Review> GetAllReviews();
    }
}

