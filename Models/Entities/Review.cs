namespace Models.Entities
{
    public class Review
    {
        public int Id { get; }
        public AccountHolder Account { get; }
        public int Rating { get;  }      //  1 to 5 stars
        public string Comment { get; }
        public decimal CleanlinessRating { get;  }
        public decimal LocationRating { get; }
        public decimal ComfortRating { get; }
        public decimal ValueRating { get; }
        public DateTime? CreatedAt { get; }

        public Review(AccountHolder account, int rating ,string comment, int id, decimal cleanlinessRating, decimal locationRating, decimal comfortRating, decimal valueRating,DateTime? createdAt)
        {
            Id = id;
            Account = account;
            Rating = rating;
            Comment = comment;
            CleanlinessRating = cleanlinessRating;
            LocationRating = locationRating;
            ComfortRating = comfortRating;
            ValueRating = valueRating;
            CreatedAt = createdAt;
        }

        

    }
}
