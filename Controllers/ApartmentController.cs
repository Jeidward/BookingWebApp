using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Microsoft.Extensions.Configuration;
using Services;

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
            // this will get  a list of apartments from the  repository/database
            return View("~/Views/Apartment/ApartmentList.cshtml", _apartmentService.GetAllApartments(3));
        }

        public IActionResult ShowApartmentPage(int id)
        {
            Apartment selectedApartment = _apartmentService.GetApartment(id);
            return View("~/Views/Apartment/Apartment.cshtml", selectedApartment);
        }
    }
}
