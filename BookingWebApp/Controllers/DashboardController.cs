using BookingWebApp.ViewModels;
using Enums;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Services;
using System.Diagnostics;
using System.Security.Claims;
using BookingWebApp.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;


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


        public async  Task<IActionResult> Index(DateTime month, int pageIndex = 1, int pageSize = 5)
        {
            bool isTrue = false;
            if (month == DateTime.MinValue) month = DateTime.Today;

            var year  = month.Year;
            var selectedMonth = (int)month.Month;

            if (selectedMonth < DateTime.Today.Month || year < DateTime.Today.Year) isTrue = true;
            
            var pageActivities = await _dashboardService.GetAllActivitiesAsync(pageIndex,pageSize);

            var activityViewModels = pageActivities.Items
                .Select(act => act.Status == BookingStatus.Confirmed ? ActivityViewModel.ConvertToViewModelForNewBooking(act)
                    : ActivityViewModel.ConvertToViewModelForBookingCancellation(act))
                .ToList();

            var analytics = _dashboardService.GetDashboardAnalytics(selectedMonth, year);

            var vmPage = PaginatedList<ActivityViewModel>.Create(activityViewModels, pageActivities.TotalItems, pageActivities.PageIndex, pageActivities.PageSize);

            var dashboardViewModel = new DashboardIndexViewModel
            {
                TotalBookings = analytics.TotalBookings,
                TotalUsers = analytics.TotalUsers,
                TotalRevenue = analytics.TotalRevenue,
                UpcomingBookings = analytics.UpcomingBookings,
                Activities = vmPage,
                IsPreviousMonth = isTrue,
                CurrentDate = month

            };
            return View(dashboardViewModel);
        }

        public IActionResult ManageApartment(int pageIndex = 1, int pageSize = 5)
        {
            var apartmentsNormal = _apartmentService.GetAllApartments();
            var viewModels = ApartmentViewModel.ConvertToViewModel(apartmentsNormal);

            var apartments = _apartmentService.GetApartmentsAsync(pageIndex, pageSize);
            var vmItems = apartments.Result.Items.Select(ApartmentViewModel.ConvertToViewModel).ToList();

            var viewModelEdit = apartments.Result.Items.Select(AddApartmentViewModel.ConvertToViewModel).ToList();
            var occupiedBookings = _dashboardService.GetOccupiedApartmentFromBookings();
            var occupiedIds = occupiedBookings.Select(b => b.ApartmentId).ToList();
            var allAmenities = AmenitiesViewModel.ConverToViewModel(_apartmentService.GetAmenitiesList());

            foreach (var vm in viewModelEdit)
                vm.Amenities = allAmenities;

            foreach (var occupiedApartment in vmItems)
            {
                if (occupiedIds.Contains(occupiedApartment.Id))
                    occupiedApartment.IsOccupied = true;
            }

            var availableToday = viewModels.Where(a => a.IsOccupied == false).ToList();
            var pagVmItems = PaginatedList<ApartmentViewModel>.Create(vmItems, apartments.Result.TotalItems, apartments.Result.PageIndex, apartments.Result.PageSize);

            var dashboard = new DashboardApartmentManagementViewModel
            {
                ApartmentViewModels = pagVmItems,
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

        public IActionResult UpcomingBookings()
        {
            var bookings = _dashboardService.UpcomingBookings();
            var bookingViewModels = bookings.Select(BookingViewModelHelper.ConvertToViewModel).ToList();

            return View(bookingViewModels);
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
