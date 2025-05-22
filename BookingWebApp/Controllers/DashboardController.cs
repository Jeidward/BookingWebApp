using BookingWebApp.ViewModels;
using Enums;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Services;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;


namespace BookingWebApp.Controllers
{
    [Authorize(Policy = "Host")]
    public class DashboardController : Controller
    {
        private readonly UserService _userService;
        private readonly AccountHolderService _accountHolderService;
        private readonly DashboardService _dashboardService;
        private readonly ApartmentService _apartmentService;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardController(UserService userService, AccountHolderService accountHolderService,
            DashboardService dashboardService, ApartmentService apartmentService,IWebHostEnvironment env,IHttpContextAccessor httpContextAccessor)
        {
            _userService = userService;
            _accountHolderService = accountHolderService;
            _dashboardService = dashboardService;
            _apartmentService = apartmentService;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        public IActionResult Index()
        {
            var activities = _dashboardService.GetAllActivities();

            var activityViewModels = new List<ActivityViewModel>();
            foreach (var activity in activities)
            {
                if (activity.Status == BookingStatus.Confirmed)
                    activityViewModels.Add(ActivityViewModel.ConvertToViewModelForNewBooking(activity));
                else if (activity.Status == BookingStatus.Cancelled)
                    activityViewModels.Add(ActivityViewModel.ConvertToViewModelForBookingCancellation(activity));
            }

            var analytics = _dashboardService.GetDashboardAnalytics();

            var dashboardViewModel = new DashboardIndexViewModel
            {
                TotalBookings = analytics.TotalBookings,
                TotalUsers = analytics.TotalUsers,
                TotalRevenue = analytics.TotalRevenue,
                UpcomingBookings = analytics.UpcomingBookings,
                Activities = activityViewModels
            };
            return View(dashboardViewModel);
        }

        public IActionResult ManageApartment()
        {
            var apartments = _apartmentService.GetAllApartments();
            var viewModels = ApartmentViewModel.ConvertToViewModel(apartments);
            var viewModelEdit = AddApartmentViewModel.ConvertToViewModel(apartments);
            var occupiedBookings = _dashboardService.GetOccupiedApartmentFromBookings();
            var occupiedIds = occupiedBookings.Select(b => b.ApartmentId).ToList();
            var allAmenities = AmenitiesViewModel.ConverToViewModel(_apartmentService.GetAmenitiesList());

            foreach (var vm in viewModelEdit)
                vm.Amenities = allAmenities;

            foreach (var occupiedApartment in viewModels)
            {
                if (occupiedIds.Contains(occupiedApartment.Id))
                    occupiedApartment.IsOccupied = true;
            }

            var availableToday = viewModels.Where(a => a.IsOccupied == false).ToList();

            var dashboard = new DashboardApartmentManagementViewModel
            {
                ApartmentViewModels = viewModels,
                ApartmentTotal = viewModels.Count,
                ApartmentCurrentlyOccupied = occupiedIds.Count,
                ApartmentAvailable = availableToday.Count,
                EditApartmentViewModels = viewModelEdit,
                Amenities = allAmenities
            };

            return View(dashboard);
        }

       
        public IActionResult AddApartment(AddApartmentViewModel apartmentViewModel)
        {
            var webRootPath = _env.WebRootPath;
            var savedNames = _dashboardService.AddImage(apartmentViewModel.NewImages,webRootPath);
            
            apartmentViewModel.ImageUrl = $"IMG/{savedNames.First()}";
            apartmentViewModel.Gallery = savedNames.Select(image => $"IMG/{image}").ToList();

            var apartment = AddApartmentViewModel.ConvertToEntity(apartmentViewModel);

            _apartmentService.AddApartment(apartment);

            return RedirectToAction("ManageApartment", "Dashboard");
        }

        public IActionResult DeleteApartment(int id)
        {
            _apartmentService.DeleteApartment(id);
            return RedirectToAction("ManageApartment", "Dashboard");
        }

      
        public IActionResult EditApartment(AddApartmentViewModel apartmentViewModel)
        {
            var finalGallery = apartmentViewModel.SelectedImages.ToList();
            if (apartmentViewModel.NewImages.Any())
            {
                var webRootPath = _env.WebRootPath;
                var savedNames = _dashboardService.AddImage(apartmentViewModel.NewImages, webRootPath);
                apartmentViewModel.ImageUrl = $"IMG/{savedNames.First()}";
                apartmentViewModel.Gallery = savedNames.Select(image => $"IMG/{image}").ToList();
                finalGallery.AddRange(apartmentViewModel.Gallery);
            }

            apartmentViewModel.Gallery = finalGallery;

            var apartment = AddApartmentViewModel.ConvertToEntity(apartmentViewModel);
            _apartmentService.UpdateApartment(apartment);

            return RedirectToAction("ManageApartment");
        }

    }

}
