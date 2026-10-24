using InventoryManagementSystemApplication.inventory.Exceptions;
using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;
using Dto = InventoryManagementSystem.Inventory.DataTransferObject;

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
            userGroup.MapPut("/suspend/{id}", SuspendUser).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            userGroup.MapPut("/unsuspend/{id}", UnsuspendUser).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListUsers([FromServices] IUserService userService,string? name,Guid? dptId,Guid? locId,int? stus)
        {
            if (stus.HasValue && (stus.Value < byte.MinValue || stus.Value > byte.MaxValue || !Enum.IsDefined(typeof(UserSuspensionState), (byte)stus.Value)))
                return Results.BadRequest(new { Message = "Invalid user status." });

            UserSuspensionState? status = stus.HasValue ? (UserSuspensionState?)stus.Value : null;

            var users = await userService.List(name, dptId, locId, status);
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.UserMapper.ToDtoList(users));
        }

        private static async Task<IResult> GetUser([FromServices] IUserService userService, Guid id)
        {
            var user = await userService.Get(id);
            if (user == null)
                return Results.NotFound();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.UserMapper.ToDto(user));
        }

        private static async Task<IResult> CreateUser(HttpContext http, [FromServices] IUserService userService, [FromServices] IDepartmentService departmentService, [FromServices] ILocationService locationService, [FromBody] Dto.User dtoUser)
        {
            Guid? agentId = GetAgentId(http);
            if (agentId == null)
                return Results.Unauthorized();

            if (await userService.GetByName(dtoUser.Name) != null)
                return Results.Conflict(new { Message = "A user with the same name already exists." });

            var department = await departmentService.Get(dtoUser.Department.Id);
            if (department == null)
                return Results.BadRequest("Department not found");

            var location = await locationService.Get(dtoUser.Location.Id);
            if (location == null)
                return Results.BadRequest("Location not found");

            DomainEntities.User user = DomainEntities.User.Create(dtoUser.Name, department, location);

            var createdUserId = await userService.Create(user, agentId.Value);
            return Results.Created($"/user/Get/{createdUserId}", new { Id = createdUserId });
        }

        private static async Task<IResult> UpdateUser(HttpContext http, [FromServices] IUserService userService, [FromServices] IDepartmentService departmentService, [FromServices] ILocationService locationService, [FromBody] Dto.User dtoUser)
        {
            Guid? agentId = GetAgentId(http);
            if (agentId == null)
                return Results.Unauthorized();

            // Without this an unknown id would update nothing yet still log an update
            if (await userService.Get(dtoUser.Id) == null)
                return Results.NotFound();

            DomainEntities.User? existingUser = await userService.GetByName(dtoUser.Name);
            if (existingUser != null && existingUser.Id != dtoUser.Id)
                return Results.Conflict(new { Message = "A user with the same name already exists." });

            var department = await departmentService.Get(dtoUser.Department.Id);
            if (department == null)
                return Results.BadRequest("Department not found");

            var location = await locationService.Get(dtoUser.Location.Id);
            if (location == null)
                return Results.BadRequest("Location not found");

            DomainEntities.User? user = DomainEntities.User.Reconstitute(dtoUser.Id, dtoUser.Name, department, location);
            if (user == null)
                return Results.BadRequest();
            await userService.Update(user, agentId.Value);
            return Results.Ok();
        }

        private static async Task<IResult> SuspendUser(HttpContext http, [FromServices] IUserService userService, Guid id)
        {
            Guid? agentId = GetAgentId(http);
            if (agentId == null)
                return Results.Unauthorized();

            var user = await userService.Get(id);
            if (user == null)
                return Results.NotFound();

            try
            {
                await userService.Suspend(id, agentId.Value);
                return Results.NoContent();
            }
            catch (UserAlreadySuspendedException ex)
            {
                return Results.Conflict(ex.Message);
            }
            catch (AssignmentAlreadyReturnedException ex)
            {
                return Results.Conflict(ex.Message);
            }
        }

        private static async Task<IResult> UnsuspendUser(HttpContext http, [FromServices] IUserService userService, Guid id)
        {
            Guid? agentId = GetAgentId(http);
            if (agentId == null)
                return Results.Unauthorized();

            var user = await userService.Get(id);
            if (user == null)
                return Results.NotFound();

            try
            {
                await userService.Unsuspend(id, agentId.Value);
                return Results.NoContent();
            }
            catch (UserNotSuspendedException ex)
            {
                return Results.Conflict(ex.Message);
            }
        }

        private static Guid? GetAgentId(HttpContext http)
        {
            string? userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out Guid agentId) ? agentId : null;
        }
    }
}
