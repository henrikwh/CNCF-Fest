﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace WebApp.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = null)
        {
            var u = User.Identity;
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        public IActionResult Signin(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Signin(string userName, string birthDate = null, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return BadRequest("A user name is required");
            }

            // In a real-world application, user credentials would need validated before signing in
            var claims = new List<Claim>();
            // Add a Name claim and, if birth date was provided, a DateOfBirth claim
            claims.Add(new Claim(ClaimTypes.Name, userName));
            if (DateTime.TryParse(birthDate, CultureInfo.InvariantCulture, out _))
            {
                claims.Add(new Claim(ClaimTypes.DateOfBirth, birthDate));
            }

            // Create user's identity and sign them in
            var identity = new ClaimsIdentity(claims, "UserSpecified");
            await HttpContext.SignInAsync(new ClaimsPrincipal(identity));

            return Redirect(returnUrl ?? "/");
        }

        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }

        public IActionResult Denied()
        {
            return View();
        }
    }

}

