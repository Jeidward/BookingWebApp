using System;
using System.Security.Claims;
using BookingWebApp.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http.Extensions;
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

            var user = UserViewModel.ConvertToEntity(userViewModel);

            var result =  _userService.Register(user); 

            if(!result.IsValid)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("error", error);
                    return View(userViewModel);
                }

            int userId = _userService.GetExistedLogIn(userViewModel.Email, userViewModel.Password);

            var claims = _userService.CreateClaims(userId, userViewModel.Email);

            var claimsIdentity = new ClaimsIdentity(claims, "User");

            HttpContext.Session.SetInt32("UserId", userId);
            bool hasAnyBooking = _accountHolderService.HasAccountHolderAnyBooking(userId);
            HttpContext.Session.SetInt32("HasBooking", hasAnyBooking ? 1 : 0);

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

            HttpContext.Session.SetInt32("UserId", userId);
            bool hasAnyBooking = _accountHolderService.HasAccountHolderAnyBooking(userId);
            HttpContext.Session.SetInt32("HasBooking", hasAnyBooking ? 1 : 0);

            var claims = _userService.CreateClaims(userId, userViewModel.Email);

            var roleClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            
            var claimsIdentityHost = new ClaimsIdentity(claims,roleClaim!.Value );

            if (_httpContextAccessor.HttpContext != null)
                await _httpContextAccessor.HttpContext.SignInAsync($"{roleClaim.Value}Scheme", new ClaimsPrincipal(claimsIdentityHost), new AuthenticationProperties());


            return RedirectToAction("Index", "Home");
        }

        public ActionResult LogOut()
        {
          
            var userId = HttpContext.Session.GetInt32("UserId");
            var user = _userService.GetUser(userId.Value);

            HttpContext.Session.Clear();
            if (user.RoleId == 2)
            {
                 _httpContextAccessor.HttpContext?.SignOutAsync("HostScheme");
                 return RedirectToAction("Index", "Home");
            }
            
            if (_httpContextAccessor.HttpContext != null)
                _httpContextAccessor.HttpContext.SignOutAsync("UserScheme");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied() // will not currently hit.
        {
            return View();
        }

    }
}
