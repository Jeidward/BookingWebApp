using System.Diagnostics;
using Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;
using BookingWebApp.ViewModels;

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
                UserViewModel user = new UserViewModel();
                return View(user);
            }
            else
            {
                User userName = _userService.GetUser(userId.Value);

                UserViewModel viewModel = UserViewModel.ConvertToViewModel(userName);

                return View(viewModel);

            }
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
