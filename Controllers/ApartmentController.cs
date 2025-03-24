using Microsoft.AspNetCore.Mvc;
using IndividualProject_BookingWebApplication.Data.Models;
using IndividualProject_BookingWebApplication.Data.Repositories;
using IndividualProject_BookingWebApplication.Logic.Services;

namespace IndividualProject_BookingWebApplication.Controllers
{
    public class ApartmentController : Controller
    {
        private readonly ApartmentService _service;

        public ApartmentController()
        {
            _service = new(WebApplication.CreateBuilder().Configuration);
        }


        public IActionResult ShowApartmentList()
        {
            // this will get  a list of apartments from the  repository/database
            return View("~/Views/Apartment/ApartmentList.cshtml", _service.GetAllApartments(3));
        }

        public IActionResult ShowApartmentPage(int id)
        {
            Apartment selectedApartment = _service.GetApartment(id);
            return View("~/Views/Apartment/Apartment.cshtml", selectedApartment);
        }
    }
}
