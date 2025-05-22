using BookingWebApp.ViewModels;

public class ReviewsPageViewModel
{
    public int ApartmentId { get; set; }
    public string ApartmentName { get; set; } = "";
    public decimal AvgRating { get; set; }
    public int ReviewsCount { get; set; }
    public PaginatedList<ReviewViewModel> ReviewPage { get; set; } = default!;

    public ReviewsPageViewModel(int id,string name, decimal avgRating,int reviewsCount,PaginatedList<ReviewViewModel> reviewPage)
    {
        ApartmentId = id;
        ApartmentName = name;
        AvgRating = avgRating;
        ReviewsCount = reviewsCount;
        ReviewPage = reviewPage;
    }






}