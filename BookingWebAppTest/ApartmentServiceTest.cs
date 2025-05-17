using Interfaces.IRepositories;
using Interfaces.IServices;
using Models.Entities;
using Moq;
using Services;

namespace BookingWebAppTest;

[TestClass]
public class ApartmentServiceTest
{

    private Mock<IApartmentRepository> _mockApartmentRepository;
    private ApartmentService _apartmentService;
    private Mock<IReviewService> _mockReviews; // the reall use of Interfaces
    private Mock<IAmenitiesRepository> _mockAmenitiesRepository;

    [TestInitialize]
    public void Setup()
    {
        _mockApartmentRepository = new Mock<IApartmentRepository>();
        _mockReviews = new Mock<IReviewService>();
        _mockAmenitiesRepository = new Mock<IAmenitiesRepository>();
        _apartmentService = new ApartmentService(_mockApartmentRepository.Object,_mockReviews.Object,_mockAmenitiesRepository.Object);
    }

    [TestMethod]
    public void UpdateApartment_ShouldExecuteUpdateFromTheRepo_WhenAValidApartmentExist() // good flow
    {
        
        var gallery = new List<string> { "IMG/Test1", "IMG/Test2" };

        var apartment = new Apartment(1,"test","test","IMG/Test",gallery,23,"papaya12",2,2);

        _mockApartmentRepository.Setup(repo => repo.Update(apartment));
        
        _apartmentService.UpdateApartment(apartment);
        
        _mockApartmentRepository.Verify(repo => repo.Update(apartment), Times.Once);

    }

    [TestMethod]
    public void UpdateApartment_ShouldThrowArgumentException_WhenApartmentIsNull() // bad flow
    {
        Apartment apartment = null;
        Assert.ThrowsException<ArgumentNullException>(() => _apartmentService.UpdateApartment(apartment));
    }

    //[TestMethod]    
    //public void AddApartment_ShouldExecuteCreateApartmentFromTheRepo_WhenAValidApartmentExist() // good flow
    //{
    //    var gallery = new List<string> { "IMG/Test1", "IMG/Test2" };
    //    var amenities = new List<Amenities> { new Amenities(1, "test", "test") };
    //    var apartment = new Apartment("test", "test", "IMG/Test", gallery, 23, "papaya12", 2, 2,amenities);
    //    _mockApartmentRepository.Setup(repo => repo.CreateApartment(apartment));
    //    _mockApartmentRepository.Setup(repo => repo.GetApartments()).Returns(new List<Apartment> { apartment });

    //    _apartmentService.AddApartment(apartment);

    //    _mockApartmentRepository.Verify(repo => repo.CreateApartment(apartment), Times.Once);
    //}

    [TestMethod]
    public void AddApartment_ShouldThrowArgumentNullException_WhenApartmentIsNull() // bad flow
    {
        Apartment apartment = null;
        Assert.ThrowsException<ArgumentNullException>(() => _apartmentService.AddApartment(apartment));
    }

    [TestMethod]
    public void AddApartment_ShouldThrowArgumentException_WhenApartmentIsInvalid() // bad flow
    {
        var apartment = new Apartment(0, "", "", "", null, 0, "", 0, 0);
        Assert.ThrowsException<ArgumentException>(() => _apartmentService.AddApartment(apartment));
    }

    [TestMethod]
    public void DeleteApartment_ShouldExecuteDeleteFromTheRepo_WhenApartmentIdIsGreaterThanZero() // good flow
    {
        var apartmentId = 1;
        _mockApartmentRepository.Setup(repo => repo.Delete(apartmentId));
        _apartmentService.DeleteApartment(apartmentId);
        _mockApartmentRepository.Verify(repo => repo.Delete(apartmentId), Times.Once);
    }

    [TestMethod]
    public void DeleteApartment_ShouldThrowArgumentException_WhenApartmentIdIsLessThanOrEqualToZero() // bad flow
    {
        var apartmentId = 0;
        Assert.ThrowsException<ArgumentException>(() => _apartmentService.DeleteApartment(apartmentId));
    }


    [TestMethod]
    public void GetAllApartments_ShouldReturnListOfApartments_WhenApartmentsExist() // good flow
    {
        var apartments = new List<Apartment>
        {
            new Apartment(1, "test", "test", "IMG/Test", null, 23, "papaya12", 2, 2),
            new Apartment(2, "test2", "test2", "IMG/Test2", null, 23, "papaya12", 2, 2)
        };
        _mockApartmentRepository.Setup(repo => repo.GetApartments()).Returns(apartments);
        _mockReviews.Setup(repo => repo.GetReviewsForApartment(It.IsAny<int>())).Returns(new List<Review>());
        var result = _apartmentService.GetAllApartments();
        Assert.AreEqual(apartments.Count, result.Count);
    }

    [TestMethod]
    public void GetAllApartments_ShouldReturnEmptyList_WhenNoApartmentsExist() // bad flow
    {
        var apartments = new List<Apartment>();
        _mockApartmentRepository.Setup(repo => repo.GetApartments()).Returns(apartments);
        var result = _apartmentService.GetAllApartments();
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void GetApartment_ShouldReturnApartment_WhenApartmentExists() // good flow
    {
        var apartmentId = 1;
        var gallery = new List<string> { "IMG/Test1", "IMG/Test2" };
        var apartment = new Apartment(1, "test", "test", "IMG/Test", gallery, 23, "papaya12", 2, 2);


        _mockApartmentRepository.Setup(repo => repo.GetApartment(apartmentId)).Returns(apartment);
        _mockReviews.Setup(repo => repo.GetReviewsForApartment(apartmentId)).Returns(new List<Review>(){new Review(new AccountHolder(1),2,"amazing stay",2,2,2,2,3,DateTime.Today)});
        _mockAmenitiesRepository.Setup(repo => repo.GetAmenities(apartmentId)).Returns(new List<Amenities>(){new Amenities(1,"josh","imgPath")});

        var result = _apartmentService.GetApartment(apartmentId);

        Assert.AreEqual(apartment, result);
    }

}
