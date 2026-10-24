using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemApplication.inventory.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;
using Dto = InventoryManagementSystem.Inventory.DataTransferObject;

namespace InventoryManagementSystem.Inventory.APIs
{
    public static class Accessory
    {
        public static void Map(WebApplication app, RouteGroupBuilder apiGroup)
        {
            RouteGroupBuilder accessoryGroup = apiGroup.MapGroup("/accessory");
            accessoryGroup.MapGet("/list", ListAccessories).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            accessoryGroup.MapGet("/Get/{id}", GetAccessory).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            accessoryGroup.MapPost("/create", CreateAccessory).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            accessoryGroup.MapPut("/update", UpdateAccessory).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            accessoryGroup.MapDelete("/delete/{id}", DeleteAccessory).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListAccessories([FromServices] IAccessoryService accessoryService, string? name, int? stock, DateTime? date)
        {
            bool? isStock = null;
            if (stock.HasValue)            
                isStock = stock != 0;
            
            var accessories = await accessoryService.List(name, isStock, date);

            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.AccessoryMapper.ToDtoList(accessories));
        }

        private static async Task<IResult> GetAccessory([FromServices] IAssignmentService assignmentService,[FromServices] IAccessoryService accessoryService, Guid id)
        {
            var accessory = await accessoryService.Get(id);
            if (accessory == null)
                return Results.NotFound();

            var dtoAccessory = InventoryManagementSystem.Inventory.Mappers.AccessoryMapper.ToDto(accessory);


            return Results.Ok(dtoAccessory);
        }

        private static async Task<IResult> CreateAccessory(HttpContext http, [FromServices] IAccessoryService accessoryService, [FromBody] Dto.Accessory dtoAccessory)
        {
            Guid? agentId = GetAgentId(http);
            if (agentId == null)
                return Results.Unauthorized();

            DomainEntities.Accessory accessory = DomainEntities.Accessory.Create(dtoAccessory.Name, dtoAccessory.Quantity,0, dtoAccessory.Date);

            var createdAccessoryId = await accessoryService.Create(accessory, agentId.Value);
            return Results.Created($"/accessory/Get/{createdAccessoryId}", new { Id = createdAccessoryId });
        }

        private static async Task<IResult> UpdateAccessory(HttpContext http, [FromServices] IAccessoryService accessoryService, [FromServices] IAssignmentService assignmentService, [FromBody] Dto.Accessory dtoAccessory)
        {
            Guid? agentId = GetAgentId(http);
            if (agentId == null)
                return Results.Unauthorized();

            DomainEntities.Accessory? existingAccessory = await accessoryService.Get(dtoAccessory.Id);
            if (existingAccessory == null)
                return Results.NotFound();

            if(existingAccessory.AssignedQuantity > dtoAccessory.Quantity)            
                return Results.BadRequest("Cannot update accessory because the new quantity is less than the quantity already assigned to assignments.");
            
            DomainEntities.Accessory? accessory = DomainEntities.Accessory.Reconstitute(dtoAccessory.Id, dtoAccessory.Name, dtoAccessory.Quantity, existingAccessory.AssignedQuantity, dtoAccessory.Date);
            if (accessory == null)
                return Results.BadRequest();

            var assignmentsWithAccessory = await assignmentService.ListByAccessoryId(dtoAccessory.Id);

            if (dtoAccessory.Quantity < assignmentsWithAccessory.Count())
            {
                return Results.BadRequest("Cannot update accessory because the new quantity is less than the quantity already assigned to assignments.");
            }

            await accessoryService.Update(accessory, agentId.Value);
            return Results.Ok();
        }

        private static async Task<IResult> DeleteAccessory(HttpContext http, [FromServices] IAccessoryService accessoryService, [FromServices] IAssignmentService assignmentService, Guid id)
        {
            Guid? agentId = GetAgentId(http);
            if (agentId == null)
                return Results.Unauthorized();

            var assignmentsWithAccessory = await assignmentService.ListByAccessoryId(id);

            if (assignmentsWithAccessory.Any())
            {
                return Results.BadRequest("Cannot delete accessory because it is assigned to one or more assignments.");
            }

            await accessoryService.Delete(id, agentId.Value);
            return Results.Ok();
        }

        private static Guid? GetAgentId(HttpContext http)
        {
            string? userId = http.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userId, out Guid agentId) ? agentId : null;
        }
    }
}
