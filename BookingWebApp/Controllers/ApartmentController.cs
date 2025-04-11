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
        
        public ApartmentController(ApartmentService apartmentService)
        {
            _apartmentService = apartmentService;
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
            ApartmentViewModel apartmentViewModel = ApartmentViewModel.ConvertToViewModel(selectedApartment);
            return View(apartmentViewModel);
        }
    }
}
