using BookingWebApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Services;


namespace BookingWebApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly AccountHolderService _accountHolderService;
        private readonly UserService _userService;


        public AuthenticationController(AccountHolderService accountHolderService, UserService userService)
        {
            _accountHolderService = accountHolderService;
            _userService = userService;
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userViewModel);
            }

            var result =  _userService.Register(userViewModel.Email, userViewModel.Password, userViewModel.Name); // new implemented.
            if(!result.IsValid)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("error", error);
                    return View(userViewModel);
                }
            int userId = _userService.GetExistedLogIn(userViewModel.Email, userViewModel.Password);

            HttpContext.Session.SetInt32("UserId", userId);
            bool hasAnyBooking = _accountHolderService.HasAccountHolderAnyBooking(userId);
            HttpContext.Session.SetInt32("HasBooking", hasAnyBooking ? 1 : 0);

            AccountHolder accountHolder = _accountHolderService.GetAccountHolderByUserId(userId); 

            if (accountHolder.Id != -1)
            {
                HttpContext.Session.SetInt32("UserId", accountHolder.Id); 
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogIn(string mail, string password)
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(NonDetailUserViewModel userViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(userViewModel);
            }

            int userId = _userService.GetExistedLogIn(userViewModel.Email, userViewModel.Password);
            if (userId == 0)
            {
                ModelState.AddModelError("Password", "Invalid password.");
                return View(userViewModel);
            }
            HttpContext.Session.SetInt32("UserId", userId);
            bool hasAnyBooking = _accountHolderService.HasAccountHolderAnyBooking(userId);
            HttpContext.Session.SetInt32("HasBooking", hasAnyBooking ? 1 : 0);

            AccountHolder accountHolder = _accountHolderService.GetAccountHolderByUserId(userId);
            
            HttpContext.Session.SetInt32("UserId", accountHolder.Id); 
            
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
