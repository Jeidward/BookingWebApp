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
        public decimal Rating { get; }
        public int ReviewsCount { get;}

        public Apartment(int id, string name, string description, string imageUrl, decimal pricePerNight, decimal rating, int reviewCount, string adress)
        {
            Id = id;
            Name = name;
            Description = description;
            ImageUrl = imageUrl;
            Adress = adress;
            PricePerNight = pricePerNight;
            Rating = rating;
            ReviewsCount = reviewCount;
            Gallery = new List<string>();
            Reviews = new List<Review>();


        }

        public void SetGallery(List<string> gallery)
        {
            Gallery = gallery;
        }

        public void SetReviews(List<Review> reviews)
        {
            Reviews = reviews;
        }
        // A list of reviews for the apartment
        //public List<Review> Reviews { get; set; } = new List<Review>();

        //// A list of date ranges representing availability
        //public List<DateRange> Availability { get; set; } = new List<DateRange>();

    }
}
