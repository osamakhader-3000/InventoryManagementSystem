using InventoryManagementSystemApplication.inventory.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Dto = InventoryManagementSystem.Inventory.DataTransferObject;
using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystem.Inventory.APIs
{
    public static class User
    {
        public static void Map(WebApplication app, RouteGroupBuilder apiGroup)
        {
            RouteGroupBuilder userGroup = apiGroup.MapGroup("/user");
            userGroup.MapGet("/list", ListUsers).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            userGroup.MapGet("/Get/{id}", GetUser).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            userGroup.MapPost("/create", CreateUser).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            userGroup.MapPut("/update", UpdateUser).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            userGroup.MapDelete("/delete/{id}", DeleteUser).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListUsers([FromServices] IUserService userService,string? name,Guid? dptId,Guid? locId)
        {
            var users = await userService.List(name, dptId, locId);
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.UserMapper.ToDtoList(users));
        }

        private static async Task<IResult> GetUser([FromServices] IUserService userService, Guid id)
        {
            var user = await userService.Get(id);
            if (user == null)
                return Results.NotFound();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.UserMapper.ToDto(user));
        }

        private static async Task<IResult> CreateUser([FromServices] IUserService userService, [FromServices] IDepartmentService departmentService, [FromServices] ILocationService locationService, [FromBody] Dto.User dtoUser)
        {
            var department = await departmentService.Get(dtoUser.Department.Id);
            if (department == null)
                return Results.BadRequest("Department not found");

            var location = await locationService.Get(dtoUser.Location.Id);
            if (location == null)
                return Results.BadRequest("Location not found");

            DomainEntities.User user = DomainEntities.User.Create(dtoUser.Name, department, location);

            var createdUserId = await userService.Create(user);
            return Results.Created($"/user/Get/{createdUserId}", new { Id = createdUserId });
        }

        private static async Task<IResult> UpdateUser([FromServices] IUserService userService, [FromServices] IDepartmentService departmentService, [FromServices] ILocationService locationService, [FromBody] Dto.User dtoUser)
        {
            var department = await departmentService.Get(dtoUser.Department.Id);
            if (department == null)
                return Results.BadRequest("Department not found");

            var location = await locationService.Get(dtoUser.Location.Id);
            if (location == null)
                return Results.BadRequest("Location not found");

            DomainEntities.User? user = DomainEntities.User.Reconstitute(dtoUser.Id, dtoUser.Name, department, location);
            if (user == null)
                return Results.BadRequest();
            await userService.Update(user);
            return Results.Ok();
        }

        private static async Task<IResult> DeleteUser([FromServices] IUserService userService, [FromServices] IAssignmentService assignmentService, [FromServices] ITrialLogService trialLogService, Guid id)
        {
            var userAssignments = await assignmentService.List(id, null, null, null, null, null, null);
            if (userAssignments.Any())
                return Results.Conflict("Cannot delete user because they have one or more assignments.");

            var userTrialLogs = await trialLogService.List(id, null, null, null, null, null, null, null);
            if (userTrialLogs.Any())
                return Results.Conflict("Cannot delete user because they have one or more trial logs.");

            await userService.Delete(id);
            return Results.NoContent();
        }
    }
}
