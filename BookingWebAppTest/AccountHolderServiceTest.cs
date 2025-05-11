using Enums;
using Interfaces;
using Models.Entities;
using Moq;
using Services;

namespace BookingWebAppTest;

[TestClass]
public class AccountHolderServiceTest
{

    private Mock<IAccountHolderRepository> _mockAccountHolderRepository;
    private AccountHolderService _accountHolderService;



    [TestInitialize]
    public void Setup()
    {
        _mockAccountHolderRepository = new Mock<IAccountHolderRepository>();
        _accountHolderService = new AccountHolderService(_mockAccountHolderRepository.Object);
    }


    [TestMethod]
    public void HasAccountHolderAnyBooking_ShouldReturnTrue_WhenBookingExist()
    {
        var booking = new Booking(1);
        _mockAccountHolderRepository.Setup(repo => repo.HasBookingForAccountHolder(booking.Id))
            .Returns(true);

      var hasBooking =  _accountHolderService.HasAccountHolderAnyBooking(booking.Id);

      Assert.IsTrue(hasBooking);
    }

    [TestMethod]
    public void HasAccountHolderAnyBooking_ShouldReturnFalse_WhenBookingDoesNotExist()
    {
        var booking = new Booking(0);
        _mockAccountHolderRepository.Setup(repo => repo.HasBookingForAccountHolder(booking.Id))
            .Returns(false);
        var hasBooking = _accountHolderService.HasAccountHolderAnyBooking(booking.Id);
        Assert.IsFalse(hasBooking);
    }
}
