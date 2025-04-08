using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Microsoft.Extensions.Configuration;
using Services;
using BookingWebApp.ViewModels;

namespace BookingWebApp.Controllers
{
    public class ApartmentController : Controller
    {
        private readonly ApartmentService _apartmentService;
        private readonly ReviewService _reviewService;

        public ApartmentController(ApartmentService apartmentService,ReviewService reviewService)
        {
            _apartmentService = apartmentService;
            _reviewService = reviewService;
        }


        public IActionResult ShowApartmentList()
        {
            List<Apartment> apartments = _apartmentService.GetAllApartments(3);
            List<ApartmentViewModel> apartmentsViewModel = ApartmentViewModel.ConvertToViewModel(apartments);
            return View(apartmentsViewModel);
        }

        public IActionResult ShowApartmentPage(int id)
        {
            Apartment selectedApartment = _apartmentService.GetApartment(id);
            var reviews = _reviewService.GetReviewsForApartment(id); 
            selectedApartment.SetReviews(reviews);
            ApartmentViewModel apartmentViewModel = ApartmentViewModel.ConvertToViewModel(selectedApartment);
            return View(apartmentViewModel);
        }
    }
}
