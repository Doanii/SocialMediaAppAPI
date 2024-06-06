using ClassLibrary;
using Dashboard.Data;
using Dashboard.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Dashboard.Controllers
{
    public class AccountsController(DashboardDbContext dashboardDbContext) : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            var user = dashboardDbContext.Users
                .Where(u => u.Email == model.Email)
                .FirstOrDefault();
            
            if (user == null || !PasswordHasher.VerifyPassword(model.Password, user.Password))
            {
                ModelState.AddModelError(string.Empty, "Ongeldige email of wachtwoord.");
                return View(model);
            }

            if (user.IsAdmin == false)
            {
                ModelState.AddModelError(string.Empty, "Gebruiker is geen beheerder.");
                return View(model);
            }

            // Store the token in a cookie
            Response.Cookies.Append("Token", user.ApiToken, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(8)
            });

            return RedirectToAction("Index", "Posts");
        }
    }
}
