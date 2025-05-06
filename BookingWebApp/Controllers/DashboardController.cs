using BookingWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Services;

namespace BookingWebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly UserService _userService;
        private readonly AccountHolderService _accountHolderService;
        private readonly DashboardService _dashboardService;

        public DashboardController(UserService userService, AccountHolderService accountHolderService, DashboardService dashboardService)
        {
            _userService = userService;
            _accountHolderService = accountHolderService;
            _dashboardService = dashboardService;
        }

        public IActionResult ShowLogin()
        {
            return View("Login");
        }
        public IActionResult Login(NonDetailUserViewModel userViewModel)
        {
            var host = HttpContext.Session.GetInt32("HostId");
            if (host != null)
            {
                return RedirectToAction("Index"); // put this so, when the host is already logged in, and he or she alter the url to go to login action, the system will know that she already login
            }

            if (!ModelState.IsValid)
            {
                return View(userViewModel);
            }

            int userId = _userService.GetExistedLogIn(userViewModel.Email, userViewModel.Password);
            var user = _userService.GetUserWithEmail(userViewModel.Email); // needed this to validate the user role

            if (userId == 0)
            {
                ModelState.AddModelError("Password", "Invalid email or password");
                return View(userViewModel);
            }

            if (user.RoleId == 1) // meaning 1 is guest
            {
                ModelState.AddModelError("Email", "It looks like this email belongs to a guest account, so you’re not authorized to access this page.");
                return View(userViewModel);
            }

            HttpContext.Session.SetInt32("UserId", userId);

            AccountHolder accountHolder = _accountHolderService.GetAccountHolderByUserId(userId);

            HttpContext.Session.SetInt32("HostId", accountHolder.Id);

            return RedirectToAction("Index","Dashboard");
        }

        public IActionResult Index()
        {
            var host = HttpContext.Session.GetInt32("HostId");
            if (host == null)
            {
                return RedirectToAction("ShowLogin");
            }

           var totalBookings = _dashboardService.GetTotalBookings();
           DashboardIndexViewModel dashboardViewModel = DashboardIndexViewModel.ConvertToViewModel(totalBookings);
            return View(dashboardViewModel);
        }
    }
    
}
