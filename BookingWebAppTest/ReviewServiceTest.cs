using Interfaces.IRepositories;
using Interfaces.IServices;
using Models.Entities;
using Moq;
using Services;

namespace BookingWebAppTest;

[TestClass]
public class ReviewServiceTest
{
    private Mock<IReviewRepository> _mockReviewRepository;
    private ReviewService _reviewService;

    [TestInitialize]
    public void Setup()
    {
        _mockReviewRepository = new Mock<IReviewRepository>();
        _reviewService = new ReviewService(_mockReviewRepository.Object);
    }
    [TestMethod]
    public void SaveReview_ShouldExecuteSaveInRepo_WhenApartmentIdIsGreaterThanZeroAndReviewObjectIsValid()
    {
        var apartmentId = 1;
        var review = new Review(new AccountHolder(1), 1,"Great place!", 1, 1,1,2,2,DateTime.Now);
        
        _reviewService.SaveReview(apartmentId,review);
        
        _mockReviewRepository.Verify(repo => repo.Save(apartmentId,review), Times.Once);
    }   


    [TestMethod]
    public void SaveReview_ShouldThrowArgumentException_WhenApartmentIdIsLessThanOrEqualToZero()
    {
        
        var apartmentId = 0;
        var review = new Review(new AccountHolder(1), 1, "Great place!", 1, 1, 1, 2, 2, DateTime.Now);

        Assert.ThrowsException<ArgumentException>(() => _reviewService.SaveReview(apartmentId, review));
    }


}
