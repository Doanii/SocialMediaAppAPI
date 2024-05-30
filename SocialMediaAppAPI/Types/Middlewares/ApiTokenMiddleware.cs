using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using SocialMediaAppAPI.Data;
using SocialMediaAppAPI.Types.Attributes;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SocialMediaAppAPI.Types.Middlewares
{
    public class ApiTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, APIDbContext dbContext)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null)
            {
                var actionDescriptor = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
                if (actionDescriptor != null)
                {
                    var methodInfo = actionDescriptor.MethodInfo;
                    var hasAttribute = methodInfo.GetCustomAttribute<ValidateApiTokenAttribute>() != null;

                    if (hasAttribute)
                    {
                        if (!context.Request.Headers.TryGetValue("ApiToken", out var extractedApiTokenValues))
                        {
                            context.Response.StatusCode = 401; // Unauthorized
                            await context.Response.WriteAsync("ApiToken is missing");
                            return;
                        }

                        var extractedApiToken = extractedApiTokenValues.FirstOrDefault();
                        if (string.IsNullOrEmpty(extractedApiToken))
                        {
                            context.Response.StatusCode = 401; // Unauthorized
                            await context.Response.WriteAsync("ApiToken is missing");
                            return;
                        }

                        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.ApiToken == extractedApiToken);

                        if (user == null)
                        {
                            context.Response.StatusCode = 401; // Unauthorized
                            await context.Response.WriteAsync("Invalid ApiToken");
                            return;
                        }

                        // Store the user in HttpContext.Items
                        context.Items["AuthenticatedUser"] = user;
                    }
                }
            }

            await _next(context);
        }
    }
}
