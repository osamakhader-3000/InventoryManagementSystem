using InventoryManagementSystemApplication.inventory.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Dto = InventoryManagementSystem.Inventory.DataTransferObject;
using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystem.Inventory.APIs
{
    public static class Assignment
    {
        public static void Map(WebApplication app, RouteGroupBuilder apiGroup)
        {
            RouteGroupBuilder assignmentGroup = apiGroup.MapGroup("/assignment");
            assignmentGroup.MapGet("/list", ListAssignments).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            assignmentGroup.MapGet("/Get/{id}", GetAssignment).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            assignmentGroup.MapPost("/create", CreateAssignment).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            assignmentGroup.MapPut("/update", UpdateAssignment).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            assignmentGroup.MapDelete("/delete/{id}", DeleteAssignment).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListAssignments([FromServices] IAssignmentService assignmentService)
        {
            var assignments = await assignmentService.List();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.AssignmentMapper.ToDtoList(assignments));
        }

        private static async Task<IResult> GetAssignment([FromServices] IAssignmentService assignmentService, long id)
        {
            var assignment = await assignmentService.Get(id);
            if (assignment == null)
                return Results.NotFound();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.AssignmentMapper.ToDto(assignment));
        }

        private static async Task<IResult> CreateAssignment(
            [FromServices] IAssignmentService assignmentService,
            [FromServices] IDeviceService deviceService,
            [FromServices] IUserService userService,
            [FromServices] IAccessoryService accessoryService,
            [FromBody] Dto.Assignment dtoAssignment)
        {
            var device = await deviceService.Get(dtoAssignment.DeviceId);
            if (device == null)
                return Results.BadRequest("Device not found");

            var assignedUser = await userService.Get(dtoAssignment.AssignedUserId);
            if (assignedUser == null)
                return Results.BadRequest("User not found");

            var accessories = await GetAccessories(accessoryService, dtoAssignment.AccessoryIds);
            if (accessories == null)
                return Results.BadRequest("Accessory not found");

            DomainEntities.Assignment assignment = DomainEntities.Assignment.Create(
                device,
                assignedUser,
                dtoAssignment.AssignmentDate,
                dtoAssignment.ReturnDate,
                accessories,
                dtoAssignment.Remarks);

            var createdAssignmentId = await assignmentService.Create(assignment);
            return Results.Created($"/assignment/Get/{createdAssignmentId}", new { Id = createdAssignmentId });
        }

        private static async Task<IResult> UpdateAssignment(
            [FromServices] IAssignmentService assignmentService,
            [FromServices] IDeviceService deviceService,
            [FromServices] IUserService userService,
            [FromServices] IAccessoryService accessoryService,
            [FromBody] Dto.Assignment dtoAssignment)
        {
            var device = await deviceService.Get(dtoAssignment.DeviceId);
            if (device == null)
                return Results.BadRequest("Device not found");

            var assignedUser = await userService.Get(dtoAssignment.AssignedUserId);
            if (assignedUser == null)
                return Results.BadRequest("User not found");

            var accessories = await GetAccessories(accessoryService, dtoAssignment.AccessoryIds);
            if (accessories == null)
                return Results.BadRequest("Accessory not found");

            DomainEntities.Assignment? assignment = DomainEntities.Assignment.Reconstitute(
                dtoAssignment.Id,
                device,
                assignedUser,
                dtoAssignment.AssignmentDate,
                dtoAssignment.ReturnDate,
                accessories,
                dtoAssignment.Remarks);
            if (assignment == null)
                return Results.BadRequest();
            await assignmentService.Update(assignment);
            return Results.NoContent();
        }

        private static async Task<IResult> DeleteAssignment([FromServices] IAssignmentService assignmentService, long id)
        {
            await assignmentService.Delete(id);
            return Results.NoContent();
        }

        private static async Task<List<DomainEntities.Accessory>?> GetAccessories(IAccessoryService accessoryService, IEnumerable<Guid> accessoryIds)
        {
            var accessories = new List<DomainEntities.Accessory>();
            foreach (var accessoryId in accessoryIds.Distinct())
            {
                var accessory = await accessoryService.Get(accessoryId);
                if (accessory == null)
                    return null;
                accessories.Add(accessory);
            }
            return accessories;
        }
    }
}
