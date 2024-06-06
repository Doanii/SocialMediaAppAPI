using Dashboard.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Dashboard.Filters
{
    public class AuthorizeUserAttribute() : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var httpContextAccessor = httpContext.RequestServices.GetService<IHttpContextAccessor>();
            var token = httpContextAccessor?.HttpContext?.Request.Cookies["Token"];

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectToActionResult("Login", "Accounts", null);
                return;
            }

            var dbContext = httpContext.RequestServices.GetService<DashboardDbContext>();

            var user = dbContext?.Users
                .Where(u => u.ApiToken == token)
                .FirstOrDefault();

            if (user == null)
            {
                httpContext.Response.Cookies.Delete("Token");
                context.Result = new RedirectToActionResult("Login", "Accounts", null);
                return;
            }

            if (user.IsAdmin != true)
            {
                httpContext.Response.Cookies.Delete("Token");
                context.Result = new RedirectToActionResult("Login", "Accounts", null);
            }
        }
    }
}
