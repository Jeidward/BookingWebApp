using Enums;
using Interfaces;
using Models.Entities;
using Moq;
using Services;
using System.Xml;// new framework

namespace BookingWebAppTest
{
    [TestClass]
    public sealed class BookingServiceTest
    {
        private Mock<IBookingRepository> _mockBookingRepository;
        private Mock<IApartmentRepository> _mockApartmentRepository;
        private Mock<IPaymentRepository> _mockPaymentRepository;
        private BookingService _bookingService;
        private PaymentService _paymentService;
        private CheckOutService _checkoutService;// should this be in a different test class?


        [TestInitialize]
        public void Setup()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();
            _mockPaymentRepository = new Mock<IPaymentRepository>();
            _mockApartmentRepository = new Mock<IApartmentRepository>();

            _paymentService = new PaymentService(_mockPaymentRepository.Object);
            _bookingService = new BookingService(_mockBookingRepository.Object, _mockApartmentRepository.Object,_paymentService);
            _checkoutService = new CheckOutService(_bookingService);

        }


        [TestMethod]
        public void FinalizePayment_ShouldSetBookingStatusToConfirmed_WhenPaymentStatusSucceed()
        {
            int bookingId = 123;
            Apartment testApartment = new(456, "apartment", "a very relaxing apartment", "Aruba.png", 30,"papaya 12", 3, 2);
            List<GuestProfile> testGuestProfile = new List<GuestProfile>
            {
                new GuestProfile(new AccountHolder(2),"Joshua","Sanchez",23,"josh@mail.com","0634565620","Netherlands","stationsplein 9k76"),
                 new GuestProfile(new AccountHolder(2),"Kevin","some",23,"kevin@mail.com","0738990","Germany","groesbeek")
            };

            Booking testBooking = new Booking(DateTime.Now, DateTime.Now.AddDays(4), 120, testApartment, testGuestProfile);
            testBooking.SetId(bookingId);
            Assert.AreEqual(BookingStatus.Pending, testBooking.Status, "booking should start with pending");

            PaymentMethod paymentMethod = PaymentMethod.VISA;

            _mockPaymentRepository.Setup(r => r.SavePayment(It.IsAny<Payment>())).Returns<Payment>(p => p);
            _bookingService.FinalizePayment(testBooking, paymentMethod);

            Assert.AreEqual(BookingStatus.Confirmed, testBooking.Status, "booking status should be set to confirmed after payment is finalized");

        }

 

        [TestMethod]
        public void CancelBooking_ShouldReturnTrue_WhenMoreThanSevenDaysBeforeCheckIn() // good flow
        {
            var bookingId = 123;
            var checkInDate = DateTime.Today.AddDays(10); // 10 days away should cancel
            _mockBookingRepository.Setup(repo => repo.CancelBooking(bookingId));

            var isCancelled =_bookingService.CancelBooking(bookingId, checkInDate);

            Assert.IsTrue(isCancelled, "Booking should be cancelled");
        }

        

        [TestMethod]
        public void CancelBooking_ShouldReturnFalse_WhenLessThanSevenDaysBeforeCheckIn()
        {
            var bookingId = 123;
            var checkInDate = DateTime.Today.AddDays(2);   // 2 days away  shouldn’t cancel
            
            var wasCancelled = _bookingService.CancelBooking(bookingId, checkInDate);

            Assert.IsFalse(wasCancelled, "A booking inside the 7-day window must not be cancelled");

        }

        [TestMethod]
        public void ProcessCheckOut_WithCheckoutDateTwoDaysFromToday_SetsStatusToCheckedOut()
        {
            var bookingId = 123;
            var checkInDate = DateTime.Today.AddDays(1);
            var  checkoutDate = DateTime.Today.AddDays(2);

            var testBooking = new Booking(bookingId, checkInDate, checkoutDate, 120, BookingStatus.Confirmed);


            _mockBookingRepository.Setup(repo => repo.GetBookingById(bookingId)).Returns(testBooking);
            _checkoutService.ProcessCheckOut(bookingId);

            Assert.AreEqual(BookingStatus.CheckedOut, testBooking.Status, "booking should have been checked out.");
        }

        [TestMethod]
        public void ProcessCheckOutDate_WithCheckoutDateFiveDaysFromToday_ShouldThrowException()
        {
            var bookingId = 123;
            var checkInDate = DateTime.Today.AddDays(1);
            var checkoutDate = DateTime.Today.AddDays(5);

            var testBooking = new Booking(bookingId, checkInDate, checkoutDate, 120, BookingStatus.Confirmed);


            _mockBookingRepository.Setup(repo => repo.GetBookingById(bookingId)).Returns(testBooking);
       
            Assert.ThrowsException<InvalidOperationException>(
                () => _checkoutService.ProcessCheckOut(bookingId),
                "Processing checkout on an invalid date must throw an InvalidOperationException");
        }



    }
}
