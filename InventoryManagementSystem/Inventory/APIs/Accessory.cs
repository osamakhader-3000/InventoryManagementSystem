using InventoryManagementSystemApplication.inventory.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Dto = InventoryManagementSystem.Inventory.DataTransferObject;
using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;

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

        private static async Task<IResult> ListAccessories([FromServices] IAccessoryService accessoryService)
        {
            var accessories = await accessoryService.List();
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

        private static async Task<IResult> UpdateAccessory([FromServices] IAccessoryService accessoryService, [FromBody] Dto.Accessory dtoAccessory)
        {
            DomainEntities.Accessory? accessory = DomainEntities.Accessory.Reconstitute(dtoAccessory.Id, dtoAccessory.Name, dtoAccessory.Quantity, dtoAccessory.Date);
            if (accessory == null)
                return Results.BadRequest();
            await accessoryService.Update(accessory);
            return Results.NoContent();
        }

        private static async Task<IResult> DeleteAccessory([FromServices] IAccessoryService accessoryService, Guid id)
        {
            await accessoryService.Delete(id);
            return Results.NoContent();
        }
    }
}
