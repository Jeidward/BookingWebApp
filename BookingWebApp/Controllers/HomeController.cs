using System.Diagnostics;
using Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using BookingWebApp.ViewModels;
using BookingWebApp.Helpers;
using Enums;

namespace BookingWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookingService _bookingService;
        private readonly UserService _userService;
        public HomeController(ILogger<HomeController> logger,BookingService bookingService, UserService userService)
        {
            _logger = logger;
            _bookingService = bookingService;
            _userService = userService; 
        }

        public IActionResult Index()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                IndexViewModel user = new IndexViewModel
                {
                    UserViewModel = new UserViewModel(),
                    BookingViewModel = new List<BookingViewModel>()
                };
                return View(user);
            }
            User userName = _userService.GetUser(userId.Value);

            bool hasCheckOuToday = this.CheckForTodayCheckOut(userId.Value);

            var userBooking = _bookingService.GetAllBookingsForUser(userId.Value);
            // Set session and view bag
            if (hasCheckOuToday == true)
            {
                HttpContext.Session.SetInt32("HasCheckOutToday", hasCheckOuToday ? 1 : 0);
                ViewBag.HasCheckOutToday = hasCheckOuToday;
            }
            
            IndexViewModel viewModel = new IndexViewModel
            {
                UserViewModel = UserViewModel.ConvertToViewModel(userName),
                BookingViewModel = BookingViewModelHelper.ConvertToViewModel(userBooking)
            };

            return View(viewModel);

        }


        public bool CheckForTodayCheckOut(int userId) // will maybe inside a service
        {
            var bookings = _bookingService.GetAllBookingsForUser(userId).FindAll(booking => booking.Status == BookingStatus.Confirmed);

            foreach (var booking in bookings)
            {
                if (booking.CheckOutDate.Date == DateTime.Today.AddDays(-2)) // && confirmed
                {
                    HttpContext.Session.SetString("booking",BookingViewModelHelper.CreateString(booking));
                    return true;
                }
            }
            return false;
        }

        public IActionResult Privacy()
        {
            return View();
        }

 
        
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
