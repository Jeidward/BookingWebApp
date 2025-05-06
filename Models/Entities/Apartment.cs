//using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Models.Entities
{
    public class Apartment
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Adress { get; }
        public string ImageUrl { get; }
        public List<string> Gallery { get; private set; }
        public List<Review> Reviews { get; private set; }
        public decimal PricePerNight { get; }
        public decimal AvgRating { get; private set; }
        public int ReviewsCount { get; private set; }

        public Apartment(int id, string name, string description, string imageUrl, decimal pricePerNight, string adress)
        {
            Id = id;
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            Adress = adress;
            PricePerNight = pricePerNight;
            Gallery = new List<string>();
            Reviews = new List<Review>();

        }

        public static Apartment DefaultApartment()
        {
            return new Apartment(0, string.Empty, string.Empty, string.Empty, 0, string.Empty);
        }

        public void SetGallery(List<string> gallery)
        {
            Gallery = gallery;
        }

        public void SetReviews(List<Review> reviews)
        {
            Reviews = reviews;
        }

        public void SetAvgRating(decimal rating)
        {
            AvgRating = rating;
        }

        public void SetReviewsCount(int reviewsCount)
        {
            ReviewsCount = reviewsCount;
        }
    }
}
