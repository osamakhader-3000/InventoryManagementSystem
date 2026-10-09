using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using SecurityApplicationPresentation.AuthorizationPolicies;

namespace SecurityApplicationPresentation.Middlewares
{
    public class CustomAuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {/*
            if (!context.User.Identity.IsAuthenticated)
            {
                {
                    var host = context.Request.Host;
                    var scheme = context.Request.Scheme;
                    var path = context.Request.Path;
                    await context.Response.WriteAsync($"PathBase={context.Request.PathBase}; Path={context.Request.Path}; RawTarget={context.Request.Headers.Host}{context.Request.PathBase}{context.Request.Path}");
                    // Console.WriteLine($"X-Forwarded-Host: {context.Request.Headers["X-Forwarded-Host"]}");
                    //   Console.WriteLine($"X-Forwarded-Proto: {context.Request.Headers["X-Forwarded-Proto"]}");
                }

            }

            
            if (context.Request.Path.StartsWithSegments("/signin-oidc"))
            {
                await next(context);
                return;
            }
            */
           
            

            if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure?.FailedRequirements.OfType<EmailVerifiedRequirement>().Any() == true)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync("{\"error\": \"email_verification_required\"}");
                return;
            }

            if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure?.FailedRequirements.OfType<ActiveAccountRequirement>().Any() == true)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync("{\"error\": \"active_account_required\"}");
                return;
            }


            if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure?.FailedRequirements.OfType<AdminLevelRequirement>().Any() == true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync("{\"error\": \"admin_level_required\"}");
                return;
            }

            if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure?.FailedRequirements.OfType<TechnicalSupportLevelRequirement>().Any() == true)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync("{\"error\": \"technical_support_level_required\"}");
                return;
            }

            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
