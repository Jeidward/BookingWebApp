namespace Models.Entities
{
    public class Review
    {
        public int Id { get; }
        public AccountHolder Account { get; }
        public int Rating { get;  }      //  1 to 5 stars
        public string Comment { get; }
        public int CleanlinessRating { get;  }
        public int LocationRating { get; }
        public int ComfortRating { get; }
        public int ValueRating { get; }
        public DateTime CreatedAt { get; }

        public Review(AccountHolder account, int rating, string comment, int id, int cleanlinessRating, int locationRating, int comfortRating, int valueRating,DateTime createdAt)
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
