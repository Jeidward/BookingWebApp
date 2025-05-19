using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Microsoft.Extensions.Configuration;
using Services;
using BookingWebApp.ViewModels;
using DTOs;

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
            List<Apartment> apartments = _apartmentService.GetAllApartments();
            List<ApartmentViewModel> apartmentsViewModel = ApartmentViewModel.ConvertToViewModel(apartments);
            return View(apartmentsViewModel);
        }

        public IActionResult ShowApartmentPage(int id)
        {
            Apartment selectedApartment = _apartmentService.GetApartment(id);
            if (selectedApartment.Id == 0 ) 
                return RedirectToAction("ErrorResult");
            ApartmentViewModel apartmentViewModel = ApartmentViewModel.ConvertToViewModel(selectedApartment);
            return View(apartmentViewModel);
        }

        public IActionResult ErrorResult()
        {
            return View();
        }
    }
}
