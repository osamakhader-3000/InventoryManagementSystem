using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SecurityApplicationLogic.Interfaces;
using SecurityApplicationPresentation.DataTransferObjects;
using SecurityApplicationPresentation.Mappers;
using SecurityApplicationPresentation.Settings;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace UsersApplicationPresentation.APIs
{
    public static class Authentication
    {
        private static SecurityApplicationPresentation.Settings.AuthenticationSettings? _authenticationSettings;

        public static void Map(WebApplication app, RouteGroupBuilder apiGroup, Microsoft.Extensions.Configuration.ConfigurationManager configurationManager)
        {
            _authenticationSettings = new AuthenticationSettings(configurationManager);
            RouteGroupBuilder authGroup = apiGroup.MapGroup("auth");
            authGroup.MapGet("/logout", Logout);
            authGroup.MapPost("/login", Login).AllowAnonymous();
            authGroup.MapPost("/createAccount", CreateLocalAccount).AllowAnonymous();
            authGroup.MapPost("/refresh", Refresh).AllowAnonymous();
            authGroup.MapGet("/microsoftLogin", MicrosoftLogin).AllowAnonymous();
            authGroup.MapPost("/microsoftLoginCallback", MicrosoftLoginCallback).RequireAuthorization("ExternalOnly");
            authGroup.MapGet("/currentUser", GetCurrentUser);
            authGroup.MapPut("/verifyEmailToken", VerifyEmailToken).AllowAnonymous();
            authGroup.MapGet("/resendVerificationEmail", ResendVerificationEmail);
            authGroup.MapGet("/test", TestEnd).AllowAnonymous();
        }

        private static async Task<IResult> TestEnd(IEventEmitter eventEmitter)
        {
            await eventEmitter.EmitEvent("UserRegistered","simple test");
            return TypedResults.Ok("Event emitted successfully");
        }


        private static async Task<Results<Ok, BadRequest<string>, UnauthorizedHttpResult>> ResendVerificationEmail(HttpContext http, IUserService usersService)
        {
            string? userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)            
                return TypedResults.Unauthorized();
            
            Guid userGuid = Guid.Parse(userId);
            SecurityApplicationCore.Models.User? user = await usersService.GetUser(userGuid);

            if (user == null)            
                return TypedResults.Unauthorized();
            
            if (user.EmailVerificationStatus == SecurityApplicationCore.Models.UserEmailVerificationStatus.Verified)            
                return TypedResults.BadRequest("Email is already verified.");
            
            await usersService.SendAccountVerificationEmail(user);
            return TypedResults.Ok();
        }


        private static async Task<Results<Ok<bool>, BadRequest<string>, InternalServerError<string>>> VerifyEmailToken(HttpContext http, IUserService usersService, [FromBody] TokenHolder token)
        {                      

            if(token is null || string.IsNullOrEmpty(token.Token))
                return TypedResults.BadRequest("Token must be provided.");

            bool result = await usersService.VerifyEmailToken(token.Token);
            if (result) 
                return TypedResults.Ok(true);
            else
                return TypedResults.InternalServerError("Email verification failed. The token may be invalid or expired.");

        }


        private static IResult GetCurrentUser(HttpContext http, IUserService usersService)
        {
            string? userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return TypedResults.Unauthorized();
            }

            string accountStatusClaim = http.User.FindFirstValue(nameof(SecurityApplicationCore.Models.UserAccountStatus)) ?? "0";
            string emailVerificationStatusClaim = http.User.FindFirstValue(nameof(SecurityApplicationCore.Models.UserEmailVerificationStatus)) ?? "0";
            string userRoleClaim = http.User.FindFirstValue("UserRole") ?? string.Empty;

            bool accountStatusParsable = byte.TryParse(accountStatusClaim, out byte accountStatus);
            bool emailVerificationStatusParsable = byte.TryParse(emailVerificationStatusClaim, out byte emailVerificationStatus);
            Guid userRole = Guid.TryParse(userRoleClaim, out Guid parsedUserRole) ? parsedUserRole : (Guid)Guid.Empty;

            SecurityApplicationPresentation.DataTransferObjects.User user = new()
            {
                ID = Guid.Parse(userId),
                Username = http.User.FindFirstValue(ClaimTypes.Name) ?? http.User.Identity?.Name ?? string.Empty,
                Email = http.User.FindFirstValue(ClaimTypes.Email) ?? string.Empty,
                AccountStatus = accountStatusParsable ? accountStatus : (byte)0,
                EmailVerificationStatus = emailVerificationStatusParsable ? emailVerificationStatus : (byte)0,
                UserRole = userRole
            };

            return TypedResults.Ok(user);
        }


        private static async Task<IResult> MicrosoftLogin(HttpContext http, IUsersTokenService usersTokenService, IUserService usersService, ICookiesService cookiesService)
        {
            return Results.Challenge(
           new Microsoft.AspNetCore.Authentication.AuthenticationProperties
           {
               RedirectUri = _authenticationSettings?.MicrosoftRedirectUri
           },
           new[] { "Microsoft" });

        }

        private static async Task<IResult> MicrosoftLoginCallback(HttpContext http, IUsersTokenService usersTokenService, IUserService usersService, ICookiesService cookiesService)
        {
            AuthenticateResult authResult = await http.AuthenticateAsync("External");
            if (!authResult.Succeeded || authResult.Principal == null)
            {
                return TypedResults.Unauthorized();
            }
            string? email = authResult.Principal.FindFirstValue(ClaimTypes.Email) ?? authResult.Principal.FindFirstValue("email") ?? authResult.Principal.FindFirstValue("preferred_username");

            if (email == null)
            {
                return TypedResults.BadRequest("Email claim not found.");
            }
                    

            string name = authResult.Principal.FindFirstValue(ClaimTypes.Name) ?? authResult.Principal.Identity?.Name ?? authResult.Principal.FindFirstValue("name") ?? string.Empty;


            SecurityApplicationCore.Models.User user = await usersService.GetOrCreateExternalUser(name, email);

            SecurityApplicationCore.Models.AccessToken? accessToken = await SecurityApplicationPresentation.Cookies.CookiesSupplier.AppendUserTokens(http.Response.Cookies, user, usersTokenService);

            await http.SignOutAsync("External");

            return TypedResults.Ok(accessToken.ExpiresAt);
        }


        private static async Task<Results<BadRequest<string>, UnauthorizedHttpResult, Ok<DateTime>,InternalServerError<string>>> Refresh(HttpContext http, IUsersTokenService usersTokenService, IUserService usersService, ICookiesService cookiesService)
        {

            var refreshToken = http.Request.Cookies["refresh_token"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return TypedResults.InternalServerError("Refresh token is missing.");

            (SecurityApplicationCore.Models.AccessToken? accessToken, SecurityApplicationCore.Models.RefreshToken? newRefreshToken) = await usersTokenService.ValidateAndRotateAsync(refreshToken);

            if (accessToken == null || newRefreshToken == null)
            {
                return TypedResults.InternalServerError("Failed to validate or rotate refresh token.");
            }

            cookiesService.AppendAccessCookie(http.Response.Cookies, accessToken);

            cookiesService.AppendRefreshCookie(http.Response.Cookies, newRefreshToken);

            return TypedResults.Ok(accessToken.ExpiresAt);

        }


        public static async Task<Results<BadRequest<string>, UnauthorizedHttpResult, InternalServerError<string>, Ok<DateTime>>> Login(HttpContext http, IUsersTokenService usersTokenService, IUserService usersService, ICookiesService cookiesService, [FromBody] LoginRequest request)
        {
            if (request.Username == null || request.Password == null)
            {
                return TypedResults.BadRequest("Username and password must be provided.");
            }

            SecurityApplicationCore.Models.User user = await usersService.GetValidatedUser(request.Username, request.Password);
            if (user is null)
            {
                return TypedResults.Unauthorized();
            }

            if(user.AccountStatus == SecurityApplicationCore.Models.UserAccountStatus.Suspended || user.AccountStatus == SecurityApplicationCore.Models.UserAccountStatus.Deleted)
            {
                return TypedResults.InternalServerError("You are suspended or deleted. Please contact support.");
            }


            SecurityApplicationCore.Models.AccessToken? accessToken = await SecurityApplicationPresentation.Cookies.CookiesSupplier.AppendUserTokens(http.Response.Cookies, user, usersTokenService);

            return TypedResults.Ok(accessToken.ExpiresAt);

        }

        public static async Task<Results<BadRequest<string>, InternalServerError<string>, Ok<DateTime>>> CreateLocalAccount(HttpContext http, ICookiesService cookiesService, IUsersTokenService usersTokenService, IUserService usersService,IEventEmitter eventEmitter, [FromBody] Account request)
        {

            if (request.Username == null || request.Password == null || request.Email == null)
            {
                return TypedResults.BadRequest("Username, password and email must be provided.");
            }

            request.Username = request.Username.Trim();
            request.Email = request.Email.Trim();


            SecurityApplicationCore.Models.User user = await usersService.CreateNewLocalAccount(request.Username, request.Email, request.Password);

            if (user == null)
            {
                return TypedResults.InternalServerError("User account creation failed");
            }

            //await usersService.SendAccountVerificationEmail(user);

            SecurityApplicationCore.Models.AccessToken? accessToken = await SecurityApplicationPresentation.Cookies.CookiesSupplier.AppendUserTokens(http.Response.Cookies, user, usersTokenService);

          // await eventEmitter.EmitEvent("UserRegistered", $"New user registered with email: {user.Email} , username: {user.Username} and id: {user.Id}");

            return TypedResults.Ok(accessToken.ExpiresAt);

        }
                

        public static async Task<IResult> Logout(HttpContext context, IUsersTokenService usersTokenService)
        {
            context.Response.Cookies.Delete("access_token");

            context.Response.Cookies.Delete("refresh_token");

            var refreshToken = context.Request.Cookies["refresh_token"];
            if (string.IsNullOrWhiteSpace(refreshToken))
                return TypedResults.Unauthorized();

            await usersTokenService.RevokeRefreshTokenByPlainText(refreshToken);

            return Results.Ok(new { message = "Logged out" });
        }



    }
}
