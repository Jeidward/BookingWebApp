using System.Diagnostics;
using Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace BookingWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly BookingService _bookingService;

        public HomeController(ILogger<HomeController> logger,BookingService bookingService)
        {
            _logger = logger;
            _bookingService = bookingService;
        }

        public IActionResult Index()
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
