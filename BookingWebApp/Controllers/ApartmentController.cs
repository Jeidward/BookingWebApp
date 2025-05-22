using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Microsoft.Extensions.Configuration;
using Services;
using BookingWebApp.ViewModels;
using DTOs;
using Microsoft.AspNetCore.Authorization;

namespace BookingWebApp.Controllers
{
    public class ApartmentController : Controller
    {
        private readonly ApartmentService _apartmentService;

        public ApartmentController(ApartmentService apartmentService)
        {
            _apartmentService = apartmentService;
        }

        public async Task<IActionResult> ShowApartmentList(int pageIndex = 1, int pageSize = 2)
        {
            var page = await _apartmentService.GetApartmentsAsync(pageIndex, pageSize);

            var vmItems = page.Items.Select(ApartmentViewModel.ConvertToViewModel).ToList();

            var vmPage = PaginatedList<ApartmentViewModel>.Create(vmItems, page.TotalItems, page.PageIndex, page.PageSize);

            return View(vmPage);
        }
        
        public  IActionResult ShowApartmentPage(int id)
        {
            var apt = _apartmentService.GetApartment(id);
            if (apt.Id == 0) return RedirectToAction("ErrorResult");

            var vm = ApartmentViewModel.ConvertToViewModel(apt);

           var reviews = vm.ReviewViewModel.Take(2).ToList();
           vm.ReviewViewModel = reviews;
           vm.ReviewsCount = vm.ReviewsCount - reviews.Count;

            return View(vm);
        }

        public async Task<IActionResult> Reviews(int id, int pageIndex = 1, int pageSize = 2)
        {
            var apt = _apartmentService.GetApartment(id);
            if ( apt.Id == 0) return RedirectToAction("ErrorResult");

            var page = await _apartmentService.GetReviewsAsync(id, pageIndex, pageSize);
            var items = page.Items.Select(ReviewViewModel.ConvertToViewModel).ToList();
            var reviewPage = PaginatedList<ReviewViewModel>.Create(items, page.TotalItems, page.PageIndex, page.PageSize);
            var vm = new ReviewsPageViewModel(apt.Id,apt.Name,apt.AvgRating ,apt.ReviewsCount,reviewPage);

            return View("ReviewsList",vm);       
        }

        public IActionResult ErrorResult()
        {
            return View();
        }
    }
}
