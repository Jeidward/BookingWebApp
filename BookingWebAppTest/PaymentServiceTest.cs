using Enums;
using Interfaces.IRepositories;
using Models.Entities;
using Moq;
using Services;

namespace BookingWebAppTest;

[TestClass]
public class PaymentServiceTest
{
    private Mock<IPaymentRepository> _mockPaymentRepository;
    private PaymentService _paymentService;

    [TestInitialize]
    public void Setup()
    {
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _paymentService = new PaymentService(_mockPaymentRepository.Object);
    }
    [TestMethod]
    public void CreatePayment_ShouldReturnAValidPayment_WhenBookingObjectIsValid()
    {
        var guestProfiles = new List<GuestProfile>
        {
            new GuestProfile(new AccountHolder(1), "", "", 2, "", "23444", "wdddd", "wedeidh")
        };
        
        var apartment = new Apartment(1, "a very relaxing apartment", "Aruba.png", 30, "papaya 12", 3, 2);
        var booking = new Booking(1, DateTime.Today, DateTime.Today.AddDays(3), 123, apartment,guestProfiles);
        decimal amount = 100;

        PaymentMethod paymentMethod = PaymentMethod.VISA;
    
        var payment = _paymentService.CreatePayment(booking, amount, paymentMethod);
      
        Assert.IsNotNull(payment);
        Assert.AreEqual(amount, payment.Amount);
        Assert.AreEqual(paymentMethod, payment.PaymentMethods);
    }

    [TestMethod]
    public void CreatePayment_ShouldThrowArgumentException_WhenApartmentIsNull()
    {
        var guestProfiles = new List<GuestProfile>
        {
            new GuestProfile(new AccountHolder(1), "", "", 2, "", "23444", "wdddd", "wedeidh")
        };
        var booking = new Booking(1, DateTime.MinValue, DateTime.MinValue, 0, null,guestProfiles);
        decimal amount = 100;
        PaymentMethod paymentMethod = PaymentMethod.VISA;
        Assert.ThrowsException<ArgumentException>(() => _paymentService.CreatePayment(booking, amount, paymentMethod));

    }
}
