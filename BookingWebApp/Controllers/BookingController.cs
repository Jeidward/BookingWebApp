using Azure.Core;
using Models.Entities;
using Enums;
using Microsoft.AspNetCore.Mvc;
using Services;
using BookingWebApp.ViewModels;
using BookingWebApp.Helpers;
using BookingWebApp.CompositeViewModels;

namespace BookingWebApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly BookingService _bookingService;
        private readonly AccountHolderService _accountHolderService;
        private readonly ApartmentService _apartmentService;


        public BookingController(BookingService bookingService, AccountHolderService accountHolderService, ApartmentService apartmentService)
        {
            _bookingService = bookingService;
            _accountHolderService = accountHolderService;
            _apartmentService = apartmentService;
        }

     
        public IActionResult CreateBooking(CreateBookingViewModel viewModel)
        {
            Apartment getApartment = _apartmentService.GetApartment(viewModel.ApartmentId);
            ApartmentViewModel apartmentViewModel = ApartmentViewModel.ConvertToViewModel(getApartment);
            
            if (!ModelState.IsValid)
            {
                return View("~/Views/Apartment/ShowApartmentPage.cshtml",apartmentViewModel);
            }
            DateTime CheckIn = viewModel.CheckInDate.Date.Add(viewModel.CheckInTime.TimeOfDay);
            DateTime CheckOut = viewModel.CheckOutDate.Date.Add(viewModel.CheckOutTime.TimeOfDay);
            
            if (_bookingService.IsOverlappingBookingExist(viewModel.ApartmentId, CheckIn, CheckOut))
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

            Apartment apartment = _apartmentService.GetApartment(viewModel.ApartmentId);

            decimal totalPrice = _bookingService.CalculateTotalPrice(viewModel.CheckInDate, viewModel.CheckOutDate, apartment);

            BookingViewModel bookingViewModel = new BookingViewModel() {CheckInDate = CheckIn,CheckOutDate = CheckOut, ApartmentId =viewModel.ApartmentId, UserId = (int)userId, TotalPrice = totalPrice};
            bookingViewModel.ApartmentViewModel = ApartmentViewModel.ConvertToViewModel(_apartmentService.GetApartment(viewModel.ApartmentId));

            ViewData["numberOfNights"] = _bookingService.ComputeNights(CheckIn,CheckOut);
            ViewData["numberOfGuests"] = viewModel.NumberOfGuests;
            HttpContext.Session.SetInt32("numberOfGuests", viewModel.NumberOfGuests);
            ViewData["currentGuestIndex"] = 1; // Starting with the first guest

            HttpContext.Session.SetString("newBooking", BookingViewModelHelper.CreateString(bookingViewModel));

            return View("CreateBooking",bookingViewModel);
        }

        [HttpPost]
        public IActionResult AddGuestProfile(GuestProfileViewModel guestProfileViewModel)
        {
            // Get the current guest index and total number of guests
            int? totalGuests = HttpContext.Session.GetInt32("numberOfGuests");

            if (totalGuests == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (Request.Form["currentGuestIndex"].ToString() == null)
            {
                return RedirectToAction("ApartmentList", "Apartment");
            }

            int currentGuestIndex = int.Parse(Request.Form["currentGuestIndex"]!);
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
                    return RedirectToAction("ApartmentList", "Apartment");
                }
                BookingViewModel booking = BookingViewModelHelper.ReadString(bookingString);

                // Increment the guest index
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
                return RedirectToAction("ApartmentList", "Apartment");
            }
            BookingViewModel booking = BookingViewModelHelper.ReadString(bookingString);

            ViewData["numberOfGuests"] = HttpContext.Session.GetInt32("numberOfGuests");
            ViewData["numberOfNights"] = _bookingService.ComputeNights(booking.CheckInDate, booking.CheckOutDate);

            return View(booking);
        }

        [HttpPost]
        public IActionResult FinalizeBooking(PaymentViewModel paymentViewModel)
        {
            string? bookingString = HttpContext.Session.GetString("newBooking");
            if (bookingString == null)
            {
                return RedirectToAction("ApartmentList", "Apartment");
            }
            BookingViewModel bookingViewModel = BookingViewModelHelper.ReadString(bookingString);

            // Get account holder info if available
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

            List<GuestProfile> guestProfiles = new List<GuestProfile>();
            int numberOfGuests = HttpContext.Session.GetInt32("numberOfGuests") ?? 1;

            GuestProfileViewModelHelper guestProfileViewModelHelper = new GuestProfileViewModelHelper(_accountHolderService);
            for (int i = 0; i < numberOfGuests; i++)
            {
                guestProfiles.Add(guestProfileViewModelHelper.ReadGuestProfileString(HttpContext.Session.GetString($"GuestProfile_{i + 1}")!));
                HttpContext.Session.Remove($"GuestProfile_{i + 1}");
            }

            Booking booking = new(bookingViewModel.CheckInDate, bookingViewModel.CheckOutDate, bookingViewModel.TotalPrice, _apartmentService.GetApartment(bookingViewModel.ApartmentId));
            booking.SetGuestProfile(guestProfiles); // this is where booking is actually neccessary, here we want to make the object whole.

            int bookingId = _bookingService.Save(booking);
            _bookingService.FinalizePayment(booking, PaymentMethod.MASTERCARD);

            
            HttpContext.Session.SetInt32("HasBooking", 1);
            HttpContext.Session.Remove("numberOfGuests");
            HttpContext.Session.Remove("newBooking");

            return RedirectToAction("BookingSuccessful", "Booking", new { bookingId = bookingId });
        }

        public IActionResult BookingSuccessful(int bookingId)
        {
            Booking booking = _bookingService.GetBookingWithApartment(bookingId);

            BookingCompositeViewModel BookingCompositeViewModel = new BookingCompositeViewModel
            {
                BookingViewModel = BookingViewModelHelper.ConvertToViewModel(booking)
            };

            return View(BookingCompositeViewModel);
        }


        public IActionResult MyBookings()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            List<Booking> bookings = _bookingService.GetAllBookingsForUser(userId.Value);

            List<BookingViewModel> bookingViewModels = BookingViewModelHelper.ConvertToViewModel(bookings);

            if(bookings.Count == 0)
            {
                HttpContext.Session.Remove("HasBooking");
            }

            ApartmentBookingCompositeViewModel apartmentBookingCompositeViewModel = new ApartmentBookingCompositeViewModel
            {
                BookingViewModel = bookingViewModels
            };

            return View(apartmentBookingCompositeViewModel);
        }


    }
}