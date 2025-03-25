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
        public ActionResult Register(string email, string password)
        {
            try
            {
                if (_userService.Register(email, password))
                {
                    return RedirectToAction("Index", "Home");
                }
                return View();


            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        public ActionResult LogIn(string mail, string password)
        {
            return View();

        }

        [HttpPost]
        public ActionResult LogInUser(string email, string password)
        {
            int userId = _userService.GetExistedLogIn(email, password);


            if (userId == 0)
            {
                ModelState.AddModelError("", "Invalid email or password");
                return View();
            }

            HttpContext.Session.SetInt32("UserId", userId);
            bool hasAnyBooking = _accountHolderService.HasAccountHolderAnyBooking(userId);
            HttpContext.Session.SetInt32("HasBooking", hasAnyBooking ? 1 : 0);

            AccountHolder? accountHolder = _accountHolderService.GetAccountHolderByUserId(userId);

            if (accountHolder != null)
            {
                HttpContext.Session.SetInt32("UserId", accountHolder.Id); //accountHolder, is a user
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}
