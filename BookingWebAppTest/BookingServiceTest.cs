using BookingWebAppTest.FakeServices;
using BookingWebAppTest.NewFolder;
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
        private FakeBookingService _fakeBookingService;
        private FakePaymentService _fakePaymentService;


        [TestInitialize]
        public void Setup()
        {
            _mockBookingRepository = new Mock<IBookingRepository>();
            _fakePaymentService = new FakePaymentService();
            _fakeBookingService = new FakeBookingService(_mockBookingRepository.Object, _fakePaymentService);
           
        }

        [TestMethod]
        public void Save_ValidBooking_ReturnsBookingId()
        {
            //arange
            int expectedbookingId = 123;
            Apartment testApartment = new(456, "apartment", "a very relaxing apartment", "Aruba.png", 30, 3, 3, "paradera");

            List<GuestProfile> testGuestProfile = new List<GuestProfile>
            {
                new GuestProfile(new AccountHolder(2),"Joshua","Sanchez",23,"josh@mail.com","0634565620","Netherlands","stationsplein 9k76"),
                 new GuestProfile(new AccountHolder(2),"Kevin","some",23,"kevin@mail.com","0738990","Germany","groesbeek")
            };

            Booking testBooking = new Booking(DateTime.Now, DateTime.Now.AddDays(4), 120, testApartment, testGuestProfile);

           
            _mockBookingRepository.Setup(repo=> repo.SaveBooking(It.IsAny<Booking>())).Returns(expectedbookingId);
           //act
           int result = _fakeBookingService.Save(testBooking);


            //assert
            Assert.AreEqual(expectedbookingId, result);
            Assert.AreEqual(expectedbookingId,testBooking.Id);

            _mockBookingRepository.Verify(repo =>
              repo.SaveBooking(It.Is<Booking>(b => b.CheckInDate == testBooking.CheckInDate &&b.CheckOutDate == testBooking.CheckOutDate && b.TotalPrice == testBooking.TotalPrice)),Times.Once);

            // Verify SaveGuestForBooking was called for each guest
            foreach (var guest in testGuestProfile)
            {
                _mockBookingRepository.Verify(repo =>
                    repo.SaveGuestForBooking(expectedbookingId, guest),
                    Times.Once);
            }

        }

        [TestMethod]    
        public void FinalizePayment_ShouldSetBookingStatusToConfirmed()
        {
            int bookingId = 123;
            Apartment testApartment = new(456, "apartment", "a very relaxing apartment", "Aruba.png", 30, 3, 3, "paradera");
            List<GuestProfile> testGuestProfile = new List<GuestProfile>
            {
                new GuestProfile(new AccountHolder(2),"Joshua","Sanchez",23,"josh@mail.com","0634565620","Netherlands","stationsplein 9k76"),
                 new GuestProfile(new AccountHolder(2),"Kevin","some",23,"kevin@mail.com","0738990","Germany","groesbeek")
            };

            Booking testBooking = new Booking(DateTime.Now, DateTime.Now.AddDays(4), 120, testApartment, testGuestProfile);
            testBooking.SetId(bookingId);
            Assert.AreEqual(BookingStatus.Pending, testBooking.Status,"booking should start with pending");

            PaymentMethod paymentMethod = PaymentMethod.VISA;
         
            Payment processPayment = new(testBooking, 300,paymentMethod, PaymentStatus.UNPAID,3788);

            _fakePaymentService.ProcessPayment(processPayment); 

            _fakeBookingService.FinalizePayment(bookingId,testBooking, paymentMethod);

            Assert.AreEqual(BookingStatus.Confirmed, testBooking.Status, "booking status should be set to confirmed after payment is finalized");
            _fakePaymentService.CreatePayment(testBooking, 300, paymentMethod);


        }

    }
}
