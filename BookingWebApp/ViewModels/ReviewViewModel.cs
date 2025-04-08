using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class ReviewViewModel
    {
        public int AccountId { get; set; }
        public int ReviewId { get; set; }
        public int OverallRating { get; set; }
        public int CleanlinessRating { get; set; }
        public int LocationRating { get; set; }
        public int ComfortRating { get; set; }
        public int ValueRating { get; set; }
        public string Comments { get; set; }
        public DateTime CreatedAt { get; set; }

        public static Review ConvertToEntity(ReviewViewModel model, AccountHolder account)
        {
            return new Review(account, model.OverallRating, model.Comments, model.ReviewId,model.CleanlinessRating, model.LocationRating, model.ComfortRating,model.ValueRating,model.CreatedAt);
        }

        public static ReviewViewModel ConvertToViewModel(Review review)
        {
            return new ReviewViewModel
            {
                AccountId = review.Account.Id,
                ReviewId = review.Id,
                OverallRating = review.Rating,
                CleanlinessRating = review.CleanlinessRating,
                LocationRating = review.LocationRating,
                ComfortRating = review.ComfortRating,
                ValueRating = review.ValueRating,
                Comments = review.Comment,
                CreatedAt = review.CreatedAt
            };
        }
        
        

    }
}
