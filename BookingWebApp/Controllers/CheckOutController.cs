using BookingWebApp.ViewModels;
using Enums;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Services;
using BookingWebApp.Helpers;

namespace BookingWebApp.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly CheckOutService _checkoutService;
        private readonly BookingService _bookingService;
        private readonly UserService _userService;
        private readonly ApartmentService _apartmentService;
        private readonly ReviewService _reviewService;
        private readonly AccountHolderService _accountHolderService;


        public CheckOutController(CheckOutService checkOutService, UserService userService, BookingService bookingService,ApartmentService apartmentService, ReviewService reviewService,AccountHolderService accountHolderService)
        {
            _checkoutService = checkOutService;
            _userService = userService;
            _bookingService = bookingService;
            _apartmentService = apartmentService;
            _reviewService = reviewService;
            _accountHolderService = accountHolderService;   
        }
        public IActionResult CheckoutReminder(string action)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            var user = _userService.GetUser(userId.Value);
            IndexViewModel viewModel = new IndexViewModel
            {
                UserViewModel = UserViewModel.ConvertToViewModel(user)
            };

            if (action == "dismiss")
            {
                ViewBag.ShowCheckOutModal = true;
            }

            return View("~/Views/Home/Index.cshtml", viewModel);

        }


        public IActionResult CheckOutForm()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            var booking = HttpContext.Session.GetString("booking");

            BookingViewModel bookingViewModel = BookingViewModelHelper.ReadModelBooking(booking);

            Apartment apartment = _apartmentService.GetApartment(bookingViewModel.ApartmentId);

            bookingViewModel.ApartmentViewModel = ApartmentViewModel.ConvertToViewModel(apartment);

            CheckOutViewModel viewModel = new CheckOutViewModel
            {
                BookingViewModel = bookingViewModel
            };

            return View(viewModel);
        }
        public IActionResult CheckOut(CheckOutFormViewModel viewModel, ReviewViewModel reviewViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.SkipReview == true)
            {
                // no review save to the database
            }

            int? userId = HttpContext.Session.GetInt32("UserId");

            var booking = HttpContext.Session.GetString("booking");

            var account = _accountHolderService.GetAccountHolderById(userId.Value);
            BookingViewModel bookingViewModel = BookingViewModelHelper.ReadModelBooking(booking);

            Apartment apartment = _apartmentService.GetApartment(bookingViewModel.ApartmentId);

            _checkoutService.ProcessCheckOut(bookingViewModel.Id);

            var reviewModel = ReviewViewModel.ConvertToEntity(reviewViewModel,account); // need to convert back to entity to save back to the database.
            
            _reviewService.SaveReview(apartment.Id,reviewModel);

            HttpContext.Session.Remove("HasCheckOutToday");

            return RedirectToAction("Index", "Home");
        }
        
        
        
    }
}
