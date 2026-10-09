using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SecurityApplicationLogic.Interfaces;
using SecurityApplicationPresentation.DataTransferObjects;
using SecurityApplicationPresentation.Mappers;
using System.Security.Claims;

namespace UsersApplicationPresentation.APIs
{
    public static class Users
    {
        public static void Map(WebApplication app, RouteGroupBuilder apiGroup)
        {
            RouteGroupBuilder userGroup = apiGroup.MapGroup("/users");
            userGroup.MapGet("/list", ListUsers).RequireAuthorization("VerifiedEmail", "ActiveAccount", "AdminLevel");
            userGroup.MapGet("/Get/{id}", GetUser).RequireAuthorization("VerifiedEmail", "ActiveAccount", "AdminLevel");
            userGroup.MapPut("/changePassword", ChangePassword);
            userGroup.MapPut("/suspend/{id}", SuspendUser).RequireAuthorization("VerifiedEmail", "ActiveAccount", "AdminLevel");
            userGroup.MapPut("/activate/{id}", Activate).RequireAuthorization("VerifiedEmail", "ActiveAccount", "AdminLevel");
            userGroup.MapPut("/changeRole", ChangeUserRole).RequireAuthorization("VerifiedEmail", "ActiveAccount", "AdminLevel");
            userGroup.MapGet("/getUserInfo", GetUserInfo);
            userGroup.MapPost("/listByIds", ListUsersByIds);
        }

        private static async Task<Ok<List<User>>> ListUsersByIds(IUserService usersService, [FromBody] List<Guid> userIds)
        {
            List<SecurityApplicationCore.Models.User> users = await usersService.ListUsersByIds(userIds);
            return TypedResults.Ok(users.MapToResource().ToList());
        }

        private static async Task<Results<Ok, InternalServerError<string>, BadRequest<string>>> ChangeUserRole(IUserService usersService, [FromBody] User user)
        {
            if(user == null || user.UserRole == null || user.ID == null || user.ID == Guid.Empty)
                return TypedResults.BadRequest("invalid user data");



            int result = await usersService.ChangeUserRole(user.ID.Value, user.UserRole.Value);

            if(result == 0)
                return TypedResults.InternalServerError("Change role failed");

            return TypedResults.Ok();
        }

        private static async Task<Results<Ok<User>, NotFound<string>, UnauthorizedHttpResult>> GetUserInfo(HttpContext context, IUserService usersService)
        {
            string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)            
                return TypedResults.Unauthorized();
            
            SecurityApplicationCore.Models.User? user = await usersService.GetUser(Guid.Parse(userId));
            if (user == null)
                return TypedResults.NotFound("user not found");

            return TypedResults.Ok<User>(user.MapToResource());
        }

        private static async Task<Results<Ok, BadRequest<string>>> Activate(IUserService usersService, [FromRoute] Guid id)
        {
            if (id == Guid.Empty)
                return TypedResults.BadRequest("invalid id");

            await usersService.ActivateUser(id);

            return TypedResults.Ok();
        }

        private static async Task<Results<Ok, BadRequest<string>>> SuspendUser(IUserService usersService, [FromRoute] Guid id)
        {
            if (id == Guid.Empty)
                return TypedResults.BadRequest("invalid id");

            await usersService.SuspendUser(id);
            return TypedResults.Ok();

        }

        private static async Task<Results<Ok, BadRequest<string>, UnauthorizedHttpResult>> ChangePassword(HttpContext context, IUserService usersService, ChangePasswordRequest changePasswordRequest)
        {
            string? userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return TypedResults.Unauthorized();
            }

            if (string.IsNullOrEmpty(changePasswordRequest.OldPassword) || string.IsNullOrEmpty(changePasswordRequest.NewPassword))
            {
                return TypedResults.BadRequest("Old password and new password must be provided.");
            }

            if (changePasswordRequest.OldPassword == changePasswordRequest.NewPassword)
            {
                return TypedResults.BadRequest("New password must be different from the old password.");
            }

            await usersService.ChangePassword(Guid.Parse(userId), changePasswordRequest.OldPassword, changePasswordRequest.NewPassword);


            return TypedResults.Ok();

        }

        public static async Task<Ok<List<User>>> ListUsers(IUserService usersService)
        {
            List<SecurityApplicationCore.Models.User> users = await usersService.ListUsers();
            return TypedResults.Ok(users.MapToResource().ToList());
        }


        public static async Task<Results<Ok<User>, NotFound<string>>> GetUser(IUserService usersService, [FromRoute] Guid id)
        {
            SecurityApplicationCore.Models.User? user = await usersService.GetUser(id);
            if (user == null)
                return TypedResults.NotFound("user not found");
            return TypedResults.Ok<User>(user.MapToResource());
        }


    }

}
