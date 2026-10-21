using InventoryManagementSystemApplication.inventory.Exceptions;
using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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
            assignmentGroup.MapPut("/relocate", RelocateAssignment).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            assignmentGroup.MapPut("/return/{id}", ReturnAssignment).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListAssignments(
            [FromServices] IAssignmentService assignmentService,
            Guid? userid,
            Guid? depid,
            Guid? lctnId,
            DateTime? purDate,
            string? model,
            string? sn,
            int? rtrnd)
        {

            bool? isReturned = null;
            if(rtrnd.HasValue)            
                isReturned = rtrnd.Value != 0;            

            var assignments = await assignmentService.List(userid, depid,lctnId, purDate, model, sn, isReturned);
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
            HttpContext http,
            [FromServices] IAssignmentService assignmentService,
            [FromServices] IDeviceService deviceService,
            [FromServices] IUserService userService,
            [FromServices] IAccessoryService accessoryService,
            [FromBody] Dto.Assignment dtoAssignment)
        {
            Guid? agentId = GetAgentId(http);
            if (agentId == null)
                return Results.Unauthorized();

            if (dtoAssignment.Device == null)
                return Results.BadRequest("Device is required");

            var device = await deviceService.Get(dtoAssignment.Device.Id);
            if (device == null)
                return Results.BadRequest("Device not found");

            if (device.IsSold == InventoryManagementSystemDomain.modules.inventory.Enums.DeviceSoldState.Sold)
                return Results.Conflict("Device is sold");

            if (device.IsTaken == InventoryManagementSystemDomain.modules.inventory.Enums.Availability.Taken)
                return Results.Conflict("Device is already assigned");

            if (dtoAssignment.AssignedUser == null)
                return Results.BadRequest("Assigned user is required");

            var assignedUser = await userService.Get(dtoAssignment.AssignedUser.Id);
            if (assignedUser == null)
                return Results.BadRequest("User not found");

            var accessories = await accessoryService.ListByIds(dtoAssignment.Accessories.Select(a => a.Id).ToList());
            if (accessories == null)
                return Results.BadRequest("Accessory not found");

            var missingAccessoryIds = dtoAssignment.Accessories.Select(a => a.Id).Except(accessories.Select(a => a.Id)).ToList();
            if (missingAccessoryIds.Any())
                return Results.BadRequest($"Accessories not found: {string.Join(", ", missingAccessoryIds)}");

            if (accessories.Any(a => a.AssignedQuantity == a.Quantity))
            {
                var outOfStockAccessories = accessories.Where(a => a.AssignedQuantity == a.Quantity).ToList();
                return Results.Conflict($"Accessories out of stock: {string.Join(", ", outOfStockAccessories)}");
            }

            DomainEntities.Assignment assignment = DomainEntities.Assignment.Create(
                device,
                assignedUser,
                DateTime.UtcNow,
                null,
                accessories,
                dtoAssignment.Remarks);

            try
            {
                var createdAssignmentId = await assignmentService.Create(assignment, agentId.Value);
                return Results.Created($"/assignment/Get/{createdAssignmentId}", new { Id = createdAssignmentId });
            }
            catch (AccessoryOutOfStockException ex)
            {
                return Results.Conflict(ex.Message);
            }
            catch (DeviceUnavailableException ex)
            {
                return Results.Conflict(ex.Message);
            }
        }


        private static async Task<IResult> UpdateAssignment(
            HttpContext http,
            [FromServices] IAssignmentService assignmentService,
            [FromServices] IDeviceService deviceService,
            [FromServices] IUserService userService,
            [FromServices] IAccessoryService accessoryService,
            [FromBody] Dto.Assignment dtoAssignment)
        {
            Guid? agentId = GetAgentId(http);
            if (agentId == null)
                return Results.Unauthorized();

            if (dtoAssignment.Device == null)
                return Results.BadRequest("Device is required");

            var device = await deviceService.Get(dtoAssignment.Device.Id);
            if (device == null)
                return Results.BadRequest("Device not found");

            if (dtoAssignment.AssignedUser == null)
                return Results.BadRequest("Assigned user is required");

            var assignedUser = await userService.Get(dtoAssignment.AssignedUser.Id);
            if (assignedUser == null)
                return Results.BadRequest("User not found");

            var accessories = await accessoryService.ListByIds(dtoAssignment.Accessories.Select(a => a.Id).ToList());
            if (accessories == null)
                return Results.BadRequest("Accessory not found");

            var missingAccessoryIds = dtoAssignment.Accessories.Select(a => a.Id).Except(accessories.Select(a => a.Id)).ToList();
            if (missingAccessoryIds.Any())
                return Results.BadRequest($"Accessories not found: {string.Join(", ", missingAccessoryIds)}");

            var existingAssignment = await assignmentService.Get(dtoAssignment.Id);

            if (existingAssignment == null)
                return Results.NotFound();

            if (existingAssignment.Activity == AssignmentActivity.Returned)
                return Results.Conflict("Assignment is already returned");

            DomainEntities.Assignment? assignment = DomainEntities.Assignment.Reconstitute(
                dtoAssignment.Id,
                device,
                assignedUser,
                existingAssignment.AssignmentDate,
                null,
                accessories,
                dtoAssignment.Remarks,
                existingAssignment.Activity);

            if (assignment == null)
                return Results.BadRequest();

            try
            {
                await assignmentService.Update(assignment, agentId.Value);
                return Results.Ok();
            }
            catch (AccessoryOutOfStockException ex)
            {
                return Results.Conflict(ex.Message);
            }
            catch (DeviceUnavailableException ex)
            {
                return Results.Conflict(ex.Message);
            }
        }

        private static async Task<IResult> RelocateAssignment(
            HttpContext http,
            [FromServices] IAssignmentService assignmentService,
            [FromServices] IUserService userService,
            [FromBody] Dto.Assignment dtoAssignment)
        {
            Guid? agentId = GetAgentId(http);
            if (agentId == null)
                return Results.Unauthorized();

            if (dtoAssignment.AssignedUser == null)
                return Results.BadRequest("Assigned user is required");

            var assignedUser = await userService.Get(dtoAssignment.AssignedUser.Id);
            if (assignedUser == null)
                return Results.BadRequest("User not found");

            var existingAssignment = await assignmentService.Get(dtoAssignment.Id);
            if (existingAssignment == null)
                return Results.NotFound();

            if (existingAssignment.Activity == AssignmentActivity.Returned)
                return Results.Conflict("Assignment is already returned");

            DomainEntities.Assignment assignment = DomainEntities.Assignment.Reconstitute(
                dtoAssignment.Id,
                existingAssignment.Device,
                assignedUser,
                existingAssignment.AssignmentDate,
                null,
                existingAssignment.Accessories,
                dtoAssignment.Remarks,
                existingAssignment.Activity);

            await assignmentService.Relocate(assignment, agentId.Value);
            return Results.Ok();
        }

        private static Guid? GetAgentId(HttpContext http)
        {
            string? userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out Guid agentId) ? agentId : null;
        }

        private static async Task<IResult> ReturnAssignment(
            HttpContext http,
            [FromServices] IAssignmentService assignmentService,
            long id)
        {
            Guid? agentId = GetAgentId(http);
            if (agentId == null)
                return Results.Unauthorized();

            var existingAssignment = await assignmentService.Get(id);
            if (existingAssignment == null)
                return Results.NotFound();

            try
            {
                await assignmentService.Return(id, agentId.Value);
                return Results.Ok();
            }
            catch (AssignmentAlreadyReturnedException ex)
            {
                return Results.Conflict(ex.Message);
            }
        }

    }
}
