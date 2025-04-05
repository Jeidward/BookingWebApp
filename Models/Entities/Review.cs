namespace Models.Entities
{
    public class Review
    {
        public int Id { get; }
        public AccountHolder Account { get; }
        public int Rating { get; private set; }      //  1 to 5 stars
        public string Comment { get; set; }

        public Review(AccountHolder account, int rating, string comment, int id)
        {
            Account = account;
            Rating = rating;
            Comment = comment;
            Id = id;
        }

        

    }
}
