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
        public int Bedrooms { get; }
        public int Bathrooms { get; }
        public  bool IsOccupied { get; private set; }
        public List<string> Gallery { get; private set; }
        public List<Amenities> Amenities { get; }
        public List<Review> Reviews { get; private set; }
        public decimal PricePerNight { get; }
        public decimal AvgRating { get; private set; }
        public int ReviewsCount { get; private set; }

        public Apartment(int id, string name, string description, string imageUrl, decimal pricePerNight, string adress, int bedrooms, int bathrooms)
        {
            Id = id;
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            Adress = adress;
            PricePerNight = pricePerNight;
            Gallery = new List<string>();
            Reviews = new List<Review>();
            Amenities = new List<Amenities>();
            Bedrooms = bedrooms;
            Bathrooms = bathrooms;
        }

        public Apartment(int id, string name, string description, string imageUrl,List<String> gallery,decimal pricePerNight, string adress, int bedrooms, int bathrooms)
        {
            Id = id;
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            Adress = adress;
            PricePerNight = pricePerNight;
            Gallery = gallery;
            Reviews = new List<Review>();
            Bedrooms = bedrooms;
            Bathrooms = bathrooms;
        }
        public static Apartment DefaultApartment()
        {
            return new Apartment(0, string.Empty, string.Empty, string.Empty, 0, string.Empty,0,0);
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
        public void SetOccupied(bool isOccupied)
        {
            IsOccupied = isOccupied;
        }
    }
}
