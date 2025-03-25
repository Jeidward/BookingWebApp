using Azure.Core;
using Models.Entities;
using Enums;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace BookingWebApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly BookingService _bookingService;
        private readonly AccountHolderService _accountHolderService;


        public BookingController(BookingService bookingService, AccountHolderService accountHolderService)
        {
            _bookingService = bookingService;
            _accountHolderService = accountHolderService;
        }

        public IActionResult CreateBooking(int numberOfGuests, DateTime checkInDate, DateTime checkOutDate, DateTime checkInTime,DateTime checkOutTime, int apartmentId)
        {
            DateTime CheckIn = checkInDate.Date.Add(checkInTime.TimeOfDay);
            DateTime CheckOut = checkOutDate.Date.Add(checkOutTime.TimeOfDay);

            int? userId = HttpContext.Session.GetInt32("UserId");
            {
                if (userId == null)
                {
                    return RedirectToAction("Login", "Authentication");
                }
            }

            Booking booking = _bookingService.CreateBooking(CheckIn, CheckOut, apartmentId);

            ViewData["numberOfGuests"] = numberOfGuests;
            HttpContext.Session.SetInt32("numberOfGuests", numberOfGuests);
            ViewData["currentGuestIndex"] = 1; // Starting with the first guest

            ViewData["numberOfNights"] = _bookingService.ComputeNights(checkInDate, checkOutDate);
            HttpContext.Session.SetString("newBooking", _bookingService.GenerateBookingString(booking));

            return View("~/Views/Apartment/BookingDetails.cshtml", booking);
        }

        [HttpPost]
        public IActionResult AddGuestProfile(
            string firstName,
            string lastName,
            int age,
            string email,
            string phoneNumber,
            string country,
            string address
        )
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
                firstName,
                lastName,
                age,
                email,
                phoneNumber,
                country,
                address
            );

            HttpContext.Session.SetString($"GuestProfile_{currentGuestIndex}", _accountHolderService.CreateGuestProfileString(profile));

            if (currentGuestIndex < totalGuests)
            {
                string? bookingString = HttpContext.Session.GetString("newBooking");
                if (bookingString == null)
                {
                    return RedirectToAction("ApartmentList", "Apartment");
                }
                Booking booking = _bookingService.ReadBookingString(bookingString);

                // Increment the guest index
                ViewData["currentGuestIndex"] = currentGuestIndex + 1;
                ViewData["numberOfGuests"] = totalGuests;
                ViewData["numberOfNights"] = _bookingService.ComputeNights(booking.CheckInDate, booking.CheckOutDate);

                return View("~/Views/Apartment/BookingDetails.cshtml", booking);
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
            Booking booking = _bookingService.ReadBookingString(bookingString);

            ViewData["numberOfGuests"] = HttpContext.Session.GetInt32("numberOfGuests");
            ViewData["numberOfNights"] = _bookingService.ComputeNights(booking.CheckInDate, booking.CheckOutDate);

            return View("~/Views/Booking/PaymentDetailsForm.cshtml", booking);
        }

        [HttpPost]
        public IActionResult FinalizeBooking(
            string cardNumber,
            int cardExpirationMonth,
            int cardExpirationYear,
            string cardCVC,
            string country,
            string address,
            string city,
            string state,
            string zipCode
        )
        {
            string? bookingString = HttpContext.Session.GetString("newBooking");
            if (bookingString == null)
            {
                return RedirectToAction("ApartmentList", "Apartment");
            }
            Booking booking = _bookingService.ReadBookingString(bookingString);


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
            for (int i = 0; i < numberOfGuests; i++)
            {
                guestProfiles.Add(_accountHolderService.ReadGuestProfileString(HttpContext.Session.GetString($"GuestProfile_{i + 1}")!));
                HttpContext.Session.Remove($"GuestProfile_{i + 1}");
            }


            Booking finalizedBooking = _bookingService.FinalizeBooking(guestProfiles, booking, booking.TotalPrice, PaymentMethod.MASTERCARD);

            HttpContext.Session.SetInt32("HasBooking", 1);
            HttpContext.Session.Remove("numberOfGuests");
            HttpContext.Session.Remove("newBooking");

            return RedirectToAction("BookingSuccessful", "Booking", new { bookingId = finalizedBooking.Id });
        }

        public IActionResult BookingSuccessful(int bookingId)
        {
            Booking booking = _bookingService.GetBookingWithApartment(bookingId);

            return View("~/Views/Booking/BookingConfirmation.cshtml", booking);
        }


        public IActionResult MyBookings()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                return RedirectToAction("Login", "Authentication");
            }

            List<Booking> bookings = _bookingService.GetAllBookingsForUser(userId.Value);

            if(bookings == null || bookings.Count == 0)
            {
                HttpContext.Session.Remove("HasBooking");
            }
            return View("MyBookings",bookings);
        }


    }
}