using System.Security.Claims;
using BookingWebApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Models.Entities;
using Services;


namespace BookingWebApp.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly AccountHolderService _accountHolderService;
        private readonly UserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AuthenticationController(AccountHolderService accountHolderService, UserService userService,IHttpContextAccessor httpContextAccessor)
        {
            _accountHolderService = accountHolderService;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
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

            var result =  _userService.Register(userViewModel.Email, userViewModel.Password, userViewModel.Name); 
            if(!result.IsValid)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("error", error);
                    return View(userViewModel);
                }


            int userId = _userService.GetExistedLogIn(userViewModel.Email, userViewModel.Password);

            var claims = _userService.CreateClaims(userId, userViewModel.Email, "User");

            var claimsIdentity = new ClaimsIdentity(claims, "User");

            HttpContext.Session.SetInt32("UserId", userId);
            bool hasAnyBooking = _accountHolderService.HasAccountHolderAnyBooking(userId);
            HttpContext.Session.SetInt32("HasBooking", hasAnyBooking ? 1 : 0);

            AccountHolder accountHolder = _accountHolderService.GetAccountHolderByUserId(userId); 

            if (accountHolder.Id != -1)
                HttpContext.Session.SetInt32("UserId", accountHolder.Id);

            if (_httpContextAccessor.HttpContext != null)
                _httpContextAccessor.HttpContext.SignInAsync("UserScheme", new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties());

            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogIn(string mail, string password)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(NonDetailUserViewModel userViewModel)
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

            var claims = _userService.CreateClaims(userId, userViewModel.Email, "User");

            var claimsIdentity = new ClaimsIdentity(claims, "User");

            HttpContext.Session.SetInt32("UserId", userId);
            bool hasAnyBooking = _accountHolderService.HasAccountHolderAnyBooking(userId);
            HttpContext.Session.SetInt32("HasBooking", hasAnyBooking ? 1 : 0);

            AccountHolder accountHolder = _accountHolderService.GetAccountHolderByUserId(userId);
            
            await _httpContextAccessor.HttpContext.SignInAsync("UserScheme", new ClaimsPrincipal(claimsIdentity),
                                                                                    new AuthenticationProperties());
            HttpContext.Session.SetInt32("UserId", accountHolder.Id);

            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            if (_httpContextAccessor.HttpContext != null)
                _httpContextAccessor.HttpContext.SignOutAsync("UserScheme");
            return RedirectToAction("Index", "Home");
        }




        //............................Host login section............................//
        public IActionResult ShowLogin()
        { 
            return View("~/Views/Dashboard/Login.cshtml");
        }


        public async Task<IActionResult> LoginHost(NonDetailUserViewModel userViewModel)
        {
            
            if (!ModelState.IsValid) return View("~/Views/Dashboard/Login.cshtml", userViewModel);
            
            int userId = _userService.GetExistedLogIn(userViewModel.Email, userViewModel.Password);
            var user = _userService.GetUserWithEmail(userViewModel.Email); 

            if (userId == 0)
            {
                ModelState.AddModelError("Password", "Invalid email or password");
                return View("~/Views/Dashboard/Login.cshtml", userViewModel);
            }

            if (user.RoleId == 1) // meaning 1 is guest
            {
                ModelState.AddModelError("Email",
                    "It looks like this email belongs to a guest account, so you’re not authorized to access this page.");
                return View("~/Views/Dashboard/Login.cshtml", userViewModel);
            }

            var claims = _userService.CreateClaims(userId, userViewModel.Email,"Host");

            var claimsIdentity = new ClaimsIdentity(claims, "Host");
            if (_httpContextAccessor.HttpContext != null)
                await _httpContextAccessor.HttpContext.SignInAsync("HostScheme", new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties());

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult LogoutHost()
        {
            if (_httpContextAccessor.HttpContext != null)
                _httpContextAccessor.HttpContext.SignOutAsync("HostScheme");
            return RedirectToAction("ShowLogin","Authentication");
        }

    }
}
