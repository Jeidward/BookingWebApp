using System.Diagnostics;
using System.Security.Claims;
using Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using BookingWebApp.ViewModels;
using BookingWebApp.Helpers;
using BookingWebApp.Hub;
using Enums;
using Interfaces.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BookingWebApp.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookingService _bookingService;
        private readonly UserService _userService;
        private readonly IReviewService _reviewService;
        private readonly ChatService _chatService;
        private readonly IHubContext<ChatHub> _hubContext;

        public HomeController(ILogger<HomeController> logger,BookingService bookingService, UserService userService, IReviewService reviewService, ChatService chatService,IHubContext<ChatHub>hubContext)
        {
            _logger = logger;
            _bookingService = bookingService;
            _userService = userService;
            _reviewService = reviewService;
            _chatService = chatService;
            _hubContext = hubContext;
        }

      
        public IActionResult Index()
        {
           var reviews =  _reviewService.GetAllReviews(); // this review still need fixing.
            int? userId = HttpContext.Session.GetInt32("UserId");

            if (userId == null)
            {
                IndexViewModel user = new IndexViewModel
                {
                    UserViewModel = new UserViewModel(),
                    BookingViewModel = new List<BookingViewModel>(),
                    ReviewViewModel = ReviewViewModel.ConvertToViewModel(reviews)

                };
                return View(user);
            }
            User userName = _userService.GetUser(userId.Value);
            

            bool hasCheckOuToday = CheckForTodayCheckOut(userId.Value);

            var userBooking = _bookingService.GetAllBookingsForUser(userId.Value);
        
            if (hasCheckOuToday)
            {
                HttpContext.Session.SetInt32("HasCheckOutToday", hasCheckOuToday ? 1 : 0);
                ViewBag.HasCheckOutToday = hasCheckOuToday;
            }

            if (userName.RoleId == 2)
                ViewBag.HostTool = true;
            
            IndexViewModel viewModel = new IndexViewModel
            {
                UserViewModel = UserViewModel.ConvertToViewModel(userName),
                BookingViewModel = BookingViewModelHelper.ConvertToViewModel(userBooking),
                ReviewViewModel = ReviewViewModel.ConvertToViewModel(reviews)
            };

            return View(viewModel);

        }


        public bool CheckForTodayCheckOut(int userId) // will maybe inside a service
        {
            var bookings = _bookingService.GetAllBookingsForUser(userId).FindAll(booking => booking.Status == BookingStatus.Confirmed);

            foreach (var booking in bookings)
            {
                if (booking.CheckOutDate.Date == DateTime.Today) // && confirmed
                {
                    HttpContext.Session.SetString("booking",BookingViewModelHelper.CreateString(booking));
                    return true;
                }
            }
            return false;
        }

        public IActionResult Chat()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "Id");
            int id = userId != null ? int.Parse(userId.Value) : 0;
            
            if (id == 0) return RedirectToAction("ContactUs", "Home");

            var chatMessages = _chatService.GetAllMessages(id).Select(m => new ChatMessageViewModel
            {
                Message = m.Content,
                TimeSent = m.TimeSent,
            }).ToList();


            return View(chatMessages);
        }

        public IActionResult ContactUs()
        {
            return View();
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
