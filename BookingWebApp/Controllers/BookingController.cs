using System.Linq.Expressions;
using Azure.Core;
using Models.Entities;
using Enums;
using Microsoft.AspNetCore.Mvc;
using Services;
using BookingWebApp.ViewModels;
using BookingWebApp.Helpers;
using BookingWebApp.CompositeViewModels;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.SignalR;
using Models.Enums;

namespace BookingWebApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly BookingService _bookingService;
        private readonly AccountHolderService _accountHolderService;
        private readonly ApartmentService _apartmentService;
        private readonly EmailSenderService _emailSenderService;
        private readonly UserService _userService;


        public BookingController(BookingService bookingService, AccountHolderService accountHolderService, ApartmentService apartmentService, EmailSenderService emailSenderService, UserService userService)
        {
            _bookingService = bookingService;
            _accountHolderService = accountHolderService;
            _apartmentService = apartmentService;
            _emailSenderService = emailSenderService;
            _userService = userService;
        }


        public IActionResult CreateBooking(CreateBookingViewModel viewModel)
        {
            Apartment apartment = _apartmentService.GetApartment(viewModel.ApartmentId);
            ApartmentViewModel apartmentViewModel = ApartmentViewModel.ConvertToViewModel(apartment);

            if (!ModelState.IsValid)
            {
                return View("~/Views/Apartment/ShowApartmentPage.cshtml", apartmentViewModel);
            }

            int? userId = HttpContext.Session.GetInt32("UserId");
            {
                if (userId == null)
                {
                    ViewBag.ShowLoginModal = true;
                    return View("~/Views/Apartment/ShowApartmentPage.cshtml", apartmentViewModel);
                }
            }

            DateTime checkIn = viewModel.CheckInDate.Date.Add(viewModel.CheckInTime.TimeOfDay);
            DateTime checkOut = viewModel.CheckOutDate.Date.Add(viewModel.CheckOutTime.TimeOfDay);

            List<ExtraService> selectedServices = new List<ExtraService>();
            if (viewModel.ExtraServiceViewModel.Pool)
                selectedServices.Add(ExtraService.POOL_RENTAL);
            if (viewModel.ExtraServiceViewModel.Laundry)
                selectedServices.Add(ExtraService.LAUNDRY_RENTAL);
            if (viewModel.ExtraServiceViewModel.CarRental)
                selectedServices.Add(ExtraService.CAR_RENTAL);


            decimal totalPrice = _bookingService.CalculateTotalPrice(viewModel.CheckInDate, viewModel.CheckOutDate, apartment, selectedServices);

            BookingViewModel bookingViewModel = new BookingViewModel() { CheckInDate = checkIn, CheckOutDate = checkOut, ApartmentId = viewModel.ApartmentId, UserId = (int)userId, TotalPrice = totalPrice, ExtraServiceViewModels = viewModel.ExtraServiceViewModel};
            bookingViewModel.ApartmentViewModel = apartmentViewModel;

            ViewData["numberOfNights"] = _bookingService.ComputeNights(checkIn, checkOut);
            ViewData["numberOfGuests"] = viewModel.NumberOfGuests;
            HttpContext.Session.SetInt32("numberOfGuests", viewModel.NumberOfGuests);

            int currentGuestIndex = HttpContext.Session.GetInt32("CurrentGuestIndex") ?? 1;
            if (HttpContext.Session.GetInt32("CurrentGuestIndex") == null)          
                HttpContext.Session.SetInt32("CurrentGuestIndex", currentGuestIndex);

            ViewData["currentGuestIndex"] = currentGuestIndex; 

            HttpContext.Session.SetString("newBooking", BookingViewModelHelper.CreateString(bookingViewModel));
            return View("CreateBooking", bookingViewModel);
        }

     
        [HttpPost]
        public IActionResult AddGuestProfile(GuestProfileViewModel guestProfileViewModel)
        {
            int? totalGuests = HttpContext.Session.GetInt32("numberOfGuests");

            if (totalGuests == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int currentGuestIndex = HttpContext.Session.GetInt32("CurrentGuestIndex") ?? 1;
            int accountHolderId = HttpContext.Session.GetInt32("UserId") ?? 0;
            AccountHolder accountHolder;

            if (accountHolderId > 0)
            {
                accountHolder = _accountHolderService.GetAccountHolderById(accountHolderId)!;
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }

            GuestProfile profile = _accountHolderService.CreateGuestProfile(
                accountHolder,
                guestProfileViewModel.FirstName,
                guestProfileViewModel.LastName,
                guestProfileViewModel.Age,
                guestProfileViewModel.Email,
                guestProfileViewModel.PhoneNumber,
                guestProfileViewModel.Country,
                guestProfileViewModel.Adress
            );

            HttpContext.Session.SetString($"GuestProfile_{currentGuestIndex}", GuestProfileViewModelHelper.CreateGuestProfileString(GuestProfileViewModel.ConvertToViewModel(profile)));

            if (currentGuestIndex < totalGuests)
            {
                string? bookingString = HttpContext.Session.GetString("newBooking");
                if (bookingString == null)
                {
                    return RedirectToAction("ShowApartmentList", "Apartment");
                }

                BookingViewModel booking = BookingViewModelHelper.ReadString(bookingString);
                booking.ApartmentViewModel = ApartmentViewModel.ConvertToViewModel(_apartmentService.GetApartment(booking.ApartmentId));
                booking.ExtraServiceViewModels = new ExtraServiceViewModel();

                HttpContext.Session.SetInt32("CurrentGuestIndex", currentGuestIndex + 1);
                ViewData["currentGuestIndex"] = currentGuestIndex + 1;
                ViewData["numberOfGuests"] = totalGuests;
                ViewData["numberOfNights"] = _bookingService.ComputeNights(booking.CheckInDate, booking.CheckOutDate);

                return View("CreateBooking", booking);
            }
            else
            {
                return RedirectToAction("PaymentDetails");
            }
        }

        public IActionResult PaymentDetails()
        {
            string? bookingString = HttpContext.Session.GetString("newBooking");
            if (bookingString == null)
            {
                return RedirectToAction("ShowApartmentList", "Apartment");
            }
            BookingViewModel booking = BookingViewModelHelper.ReadString(bookingString);

            ViewData["numberOfGuests"] = HttpContext.Session.GetInt32("numberOfGuests");
            ViewData["numberOfNights"] = _bookingService.ComputeNights(booking.CheckInDate, booking.CheckOutDate);

            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> FinalizeBooking(PaymentViewModel paymentViewModel)
        {
            string? bookingString = HttpContext.Session.GetString("newBooking");
            if (bookingString == null)
            {
                return RedirectToAction("ShowApartmentList", "Apartment");
            }
            BookingViewModel bookingViewModel = BookingViewModelHelper.ReadString(bookingString);

            // Get account holder info if available
            int accountHolderId = HttpContext.Session.GetInt32("UserId") ?? 0;
            var user = _userService.GetUser(accountHolderId); // use for the email sender

            if (accountHolderId > 0)
            {
               var accountHolder = _accountHolderService.GetAccountHolderById(accountHolderId)!;
            }
            else
            {
                return RedirectToAction("Login", "Authentication");
            }

            List<GuestProfile> guestProfiles = new List<GuestProfile>();
            int numberOfGuests = HttpContext.Session.GetInt32("numberOfGuests") ?? 1;

            GuestProfileViewModelHelper guestProfileViewModelHelper = new GuestProfileViewModelHelper(_accountHolderService);
            for (int i = 0; i < numberOfGuests; i++)
            {
                guestProfiles.Add(guestProfileViewModelHelper.ReadGuestProfileString(HttpContext.Session.GetString($"GuestProfile_{i + 1}")!));
                HttpContext.Session.Remove($"GuestProfile_{i + 1}");
            }

            Booking booking = new(bookingViewModel.CheckInDate, bookingViewModel.CheckOutDate, bookingViewModel.TotalPrice, _apartmentService.GetApartment(bookingViewModel.ApartmentId));
            booking.SetGuestProfile(guestProfiles);

            int bookingId = _bookingService.Save(booking);
            _bookingService.FinalizePayment(booking, PaymentMethod.MASTERCARD);

            HttpContext.Session.SetInt32("HasBooking", 1);
            HttpContext.Session.Remove("numberOfGuests");
            HttpContext.Session.Remove("newBooking");

            //email sending section//
            var reciever = user.Email;
            var subject = "Booking Confirmation";
            var body = $"Dear {user.Name},\n\nYour booking has been confirmed!\n\nBooking ID: {bookingId}\nCheck-in Date: {booking.CheckInDate:MMMM dd, yyyy}\nCheck-out Date: {booking.CheckOutDate:MMMM dd, yyyy}\nTotal Payment: {booking.TotalPrice:C}\n\nThank you for choosing us!\n\nBest regards,\nThe Booking Team";

           await _emailSenderService.SendEmail(reciever, subject, body);

            return RedirectToAction("BookingSuccessful", "Booking", new { bookingId = bookingId });
        }

      
        public IActionResult BookingSuccessful(int bookingId)
        {
            Booking booking = _bookingService.GetBookingWithApartment(bookingId);

            BookingCompositeViewModel bookingCompositeViewModel = new BookingCompositeViewModel
            {
                BookingViewModel = BookingViewModelHelper.ConvertToViewModel(booking)
            };

            return View(bookingCompositeViewModel);
        }


        public IActionResult MyBookings()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            List<Booking> bookings = _bookingService.GetAllBookingForUserCurrent(userId.Value);
            
            var checkoutBooking = _bookingService.GetAllBookingsForUserCheckout(userId.Value);

            List<BookingViewModel> bookingViewModels = BookingViewModelHelper.ConvertToViewModel(bookings);
            var checkoutViewModel = BookingViewModelCheckout.ConvertToViewModel(checkoutBooking);

            ApartmentBookingCompositeViewModel apartmentBookingCompositeViewModel = new ApartmentBookingCompositeViewModel
            {
                BookingViewModel = bookingViewModels,
                BookingViewModelCheckouts = checkoutViewModel
            };

            return View(apartmentBookingCompositeViewModel);
        }

        public IActionResult ShowCancelBookingWarning(int bookingId)
        {
            TempData["CancelBookingWarning"] = true;
            TempData["BookingIdToCancel"] = bookingId;

            return RedirectToAction("MyBookings");
        }
        public IActionResult CancelBooking(int bookingId)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            List<Booking> bookings = _bookingService.GetAllBookingForUserCurrent(userId.Value);
            if (!bookings.Any(b => b.Id == bookingId))
            {
                throw new Exception("Booking not found or does not belong to the user.");
            }
            var booking = _bookingService.GetBookingWithApartment(bookingId);
            var hasCancel = _bookingService.CancelBooking(bookingId,booking.CheckInDate);
            
            if (hasCancel == false)
            {
                TempData["ShowCannotCancelModal"] = true;
                return RedirectToAction("MyBookings");
            }

            return RedirectToAction("MyBookings");

        }
    }
}