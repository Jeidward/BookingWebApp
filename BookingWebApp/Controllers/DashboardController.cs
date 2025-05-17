using BookingWebApp.ViewModels;
using Enums;
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
        private readonly ApartmentService _apartmentService;
        private readonly IWebHostEnvironment _env;  

        public DashboardController(UserService userService, AccountHolderService accountHolderService,
            DashboardService dashboardService, ApartmentService apartmentService,IWebHostEnvironment env)
        {
            _userService = userService;
            _accountHolderService = accountHolderService;
            _dashboardService = dashboardService;
            _apartmentService = apartmentService;
            _env = env;
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
                return RedirectToAction("Index"); // put this so, when the host is already logged in, and he or she alter the url to go to login action, the system will know that she already log in
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
                ModelState.AddModelError("Email",
                    "It looks like this email belongs to a guest account, so you’re not authorized to access this page.");
                return View(userViewModel);
            }

            AccountHolder accountHolder = _accountHolderService.GetAccountHolderByUserId(userId);

            HttpContext.Session.SetInt32("HostId", accountHolder.Id);

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("HostId");
            return RedirectToAction("ShowLogin");
        }

        public IActionResult Index()
        {
            var host = HttpContext.Session.GetInt32("HostId");
            if (host == null)
            {
                return RedirectToAction("ShowLogin");
            }

            var dashboardAnalytics = _dashboardService.GetDashboardAnalytics(); // way cleaner
            DashboardIndexViewModel dashboardViewModel = DashboardIndexViewModel.ConvertToViewModel(dashboardAnalytics);
            return View(dashboardViewModel);
        }


        //manage apartment section//

        public IActionResult ManageApartment()
        {
            var host = HttpContext.Session.GetInt32("HostId");
            if (host == null)
                return RedirectToAction("ShowLogin");

            var apartments = _apartmentService.GetAllApartments();
            var viewModels = ApartmentViewModel.ConvertToViewModel(apartments);
            var viewModelEdit = AddApartmentViewModel.ConvertToViewModel(apartments);
            var occupiedBookings = _dashboardService.GetOccupiedApartmentFromBookings();
            var occupiedIds = occupiedBookings.Select(b => b.ApartmentId).ToList();

            foreach (var occupiedApartment in viewModels)
            {
                if (occupiedIds.Contains(occupiedApartment.Id))
                    occupiedApartment.IsOccupied = true;
            }

            var availableToday = _dashboardService.GetAvailableApartments(DateTime.Today, DateTime.Today);

            var dashboard = new DashboardApartmentManagementViewModel
            {
                ApartmentViewModels = viewModels,
                ApartmentTotal = viewModels.Count,
                ApartmentCurrentlyOccupied = occupiedIds.Count,
                ApartmentAvailable = availableToday.Count,
                EditApartmentViewModels = viewModelEdit,
                Amenities = AmenitiesViewModel.ConverToViewModel(_apartmentService.GetAmenitiesList()),
            };

            return View(dashboard);
        }

        public IActionResult AddApartment(AddApartmentViewModel apartmentViewModel, IFormFile[] Gallery)
        {
            var host = HttpContext.Session.GetInt32("HostId");
            if (host == null)
            {
                return RedirectToAction("ShowLogin");
            }

            var webRootPath = _env.WebRootPath;
            var savedNames = _dashboardService.AddImage(Gallery,webRootPath);
            
            apartmentViewModel.ImageUrl = $"IMG/{savedNames.First()}";
            apartmentViewModel.Gallery = savedNames.Select(image => $"IMG/{image}").ToList();

            var apartment = AddApartmentViewModel.ConvertToEntity(apartmentViewModel);

            _apartmentService.AddApartment(apartment);

            return RedirectToAction("ManageApartment", "Dashboard");
        }

        public IActionResult DeleteApartment(int id)
        {
            var host = HttpContext.Session.GetInt32("HostId");
            if (host == null)
                return RedirectToAction("ShowLogin");

            _apartmentService.DeleteApartment(id);
            return RedirectToAction("ManageApartment", "Dashboard");
        }

        public IActionResult EditApartment(int Id, AddApartmentViewModel apartmentViewModel)
        {
            var apartment  =  AddApartmentViewModel.ConvertToEntity(apartmentViewModel);
            _apartmentService.UpdateApartment(apartment);

            return RedirectToAction("ManageApartment");
        }
    }

}
