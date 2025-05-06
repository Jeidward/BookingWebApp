using Models.Entities;

namespace BookingWebApp.ViewModels
{
    public class ReviewViewModel
    {
        public int AccountId { get; set; }
        public int ReviewId { get; set; }
        public int OverallRating { get; set; }
        public decimal CleanlinessRating { get; set; }
        public decimal LocationRating { get; set; }
        public decimal ComfortRating { get; set; }
        public decimal ValueRating { get; set; }
        public string? Comments { get; set; } // i allow null
        public DateTime? CreatedAt { get; set; }

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

        public static List<ReviewViewModel> ConvertToViewModel(List<Review> reviews)
        {
            List<ReviewViewModel> reviewViewModels = new List<ReviewViewModel>();
            foreach (var review in reviews)
            {
                reviewViewModels.Add(ConvertToViewModel(review));
            }
            return reviewViewModels;
        }



    }
}
