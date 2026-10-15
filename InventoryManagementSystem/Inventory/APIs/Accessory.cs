using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemApplication.inventory.Services;
using Microsoft.AspNetCore.Mvc;
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

        private static async Task<IResult> ListAccessories([FromServices] IAccessoryService accessoryService,string? name,int? stock)
        {
            var accessories = await accessoryService.List(name,stock==0?false:true);
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.AccessoryMapper.ToDtoList(accessories));
        }

        private static async Task<IResult> GetAccessory([FromServices] IAccessoryService accessoryService, Guid id)
        {
            var accessory = await accessoryService.Get(id);
            if (accessory == null)
                return Results.NotFound();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.AccessoryMapper.ToDto(accessory));
        }

        private static async Task<IResult> CreateAccessory([FromServices] IAccessoryService accessoryService, [FromBody] Dto.Accessory dtoAccessory)
        {
            DomainEntities.Accessory accessory = DomainEntities.Accessory.Create(dtoAccessory.Name, dtoAccessory.Quantity, dtoAccessory.Date);

            var createdAccessoryId = await accessoryService.Create(accessory);
            return Results.Created($"/accessory/Get/{createdAccessoryId}", new { Id = createdAccessoryId });
        }

        private static async Task<IResult> UpdateAccessory([FromServices] IAccessoryService accessoryService, [FromServices] IAssignmentService assignmentService, [FromBody] Dto.Accessory dtoAccessory)
        {
            DomainEntities.Accessory? accessory = DomainEntities.Accessory.Reconstitute(dtoAccessory.Id, dtoAccessory.Name, dtoAccessory.Quantity, dtoAccessory.Date);
            if (accessory == null)
                return Results.BadRequest();

            var assignmentsWithAccessory = await assignmentService.ListByAccessoryId(dtoAccessory.Id);

            if(dtoAccessory.Quantity < assignmentsWithAccessory.Count())
            {
                return Results.BadRequest("Cannot update accessory because the new quantity is less than the quantity already assigned to assignments.");
            }

            await accessoryService.Update(accessory);
            return Results.Ok();
        }

        private static async Task<IResult> DeleteAccessory([FromServices] IAccessoryService accessoryService, [FromServices] IAssignmentService assignmentService, Guid id)
        {
            var assignmentsWithAccessory = await assignmentService.ListByAccessoryId(id);

            if(assignmentsWithAccessory.Any())
            {
                return Results.BadRequest("Cannot delete accessory because it is assigned to one or more assignments.");
            }

            await accessoryService.Delete(id);
            return Results.Ok();
        }
    }
}
