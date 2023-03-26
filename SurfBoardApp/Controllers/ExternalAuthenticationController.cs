// This code file defines the ExternalAuthenticationController class in the SurfBoardApp.Controllers namespace
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SurfBoardApp.Data.Models;
using System;
using System.Security.Claims;

namespace SurfBoardApp.Controllers
{
    public class ExternalAuthenticationController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        // Action method to display the Index view
        private readonly UserManager<ApplicationUser> _userManager; //dette field instansieres ingen steder. Bør injectes i constructoren.
        public IActionResult Index()
        {
            return View();
        }

        // Action method to initiate Google authentication
        public IActionResult Google()
        {
            var properties = new AuthenticationProperties { RedirectUri = "https://localhost/" };
            return Challenge(properties, "Google");
        }

        // Action method to handle the Google authentication callback
        public async Task<IActionResult> GoogleCallback()
        {
            var result = await HttpContext.AuthenticateAsync("ExternalCookie");

            if (!result.Succeeded)
            {
                // Handle the case where external authentication failed
                return RedirectToAction(nameof(Index));
            }

            // External authentication succeeded, sign in the user
            var claims = result.Principal.Claims;
            var user = new ApplicationUser
            {
                Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                FirstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                LastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
            };

            await _userManager.CreateAsync(user);

            var identity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
        }, CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal, result.Properties);

            return RedirectToAction("Index", "Home");
        }
    }
}