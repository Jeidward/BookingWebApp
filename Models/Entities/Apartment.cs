//using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace Models.Entities
{
    public class Apartment
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }
        public string Adress { get; }
        public string FirstImage { get; private set; }
        public int Bedrooms { get; }
        public int Bathrooms { get; }
        public  bool IsOccupied { get; private set; }
        public List<string> Gallery { get; private set; }
        public List<Amenities> Amenities { get; } = new List<Amenities>(); // added this for the test other ways it will be null
        public List<Review> Reviews { get; private set; }
        public decimal PricePerNight { get; }
        public decimal AvgRating { get; private set; }
        public int ReviewsCount { get; private set; }

        public Apartment(int id, string name, string description, decimal pricePerNight, string adress, int bedrooms, int bathrooms)
        {
            Id = id;
            Name = name;
            Description = description;
            Adress = adress;
            PricePerNight = pricePerNight;
            Gallery = new List<string>();
            Reviews = new List<Review>();
            Amenities = new List<Amenities>();
            Bedrooms = bedrooms;
            Bathrooms = bathrooms;
        }

        public Apartment(int id, string name, string description, string firstImage,List<string> gallery,decimal pricePerNight, string adress, int bedrooms, int bathrooms)
        {
            Id = id;
            Name = name;
            Description = description;
            FirstImage = firstImage;
            Adress = adress;
            PricePerNight = pricePerNight;
            Gallery = gallery;
            Reviews = [];
            Bedrooms = bedrooms;
            Bathrooms = bathrooms;
        }

        public Apartment(int id,string name, string description, string firstImage,List<Amenities>amenities,List<string> gallery, decimal pricePerNight, string adress, int bedrooms, int bathrooms) // constructor for creating an apartment with amenities/ use now for testing but later will be used for creating an apartment with amenities Line 37 will(constructor)
        {
            Id = id;
            Name = name;
            Description = description;
            FirstImage = firstImage;
            Adress = adress;
            PricePerNight = pricePerNight;
            Gallery = gallery;
            Reviews = new List<Review>();
            Amenities = amenities;
            Bedrooms = bedrooms;
            Bathrooms = bathrooms;
        }
        public static Apartment DefaultApartment()
        {
            return new Apartment(0, string.Empty, string.Empty, 0, string.Empty,0,0);
        }

        public void SetGallery(List<string> gallery)
        {
            Gallery = gallery;
        }

        public void SetFirstImage(string firstImage)
        {
            FirstImage = firstImage;
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

        public void SetAmenities(List<Amenities> amenities)
        {
            foreach (var amenity in amenities) // to list wasn't there.
            {
                Amenities.Add(amenity);
            }
        }

        
    }
}
