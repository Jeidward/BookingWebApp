using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using Enums;
using Interfaces.IRepositories;
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
        var booking = new Booking(999);
        _mockAccountHolderRepository.Setup(repo => repo.HasBookingForAccountHolder(booking.Id))
            .Returns(false);
        var hasBooking = _accountHolderService.HasAccountHolderAnyBooking(booking.Id);
        Assert.IsFalse(hasBooking);
    }

    [TestMethod]
    public void HasAccountHolderAnyBooking_ShouldThrowException_WhenBookingIsNotValid()
    {
        var booking = new Booking(id: 0);
        Assert.ThrowsException<ArgumentException>(() => _accountHolderService.HasAccountHolderAnyBooking(booking.Id));
    }

    [TestMethod]
    public void GetAccountHolderByUserId_ShouldThrowException_WheneUserIdIsLessOrEqualToZero()
    {
        var accountHolder = new AccountHolder( 0);
        Assert.ThrowsException<ArgumentException>(() => _accountHolderService.GetAccountHolderByUserId(accountHolder.Id));
    }

    [TestMethod]
    public void CreateGuestProfile_ShouldThrowExeption_WhenGuestProfileIsNotAValidObjectForCreation()
    {
        var guest = new GuestProfile(new AccountHolder(0), "joshua", "sanchez", 0, "", "", "", "");

        Assert.ThrowsException<ArgumentException>(() => _accountHolderService.CreateGuestProfile(guest));
    }
}
