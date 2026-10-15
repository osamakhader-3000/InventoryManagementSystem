using InventoryManagementSystemApplication.inventory.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Dto = InventoryManagementSystem.Inventory.DataTransferObject;
using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystem.Inventory.APIs
{
    public static class Location
    {
        public static void Map(WebApplication app, RouteGroupBuilder apiGroup)
        {
            RouteGroupBuilder locationGroup = apiGroup.MapGroup("/location");
            locationGroup.MapGet("/list", ListLocations).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            locationGroup.MapGet("/Get/{id}", GetLocation).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            locationGroup.MapPost("/create", CreateLocation).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            locationGroup.MapPut("/update", UpdateLocation).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            locationGroup.MapDelete("/delete/{id}", DeleteLocation).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListLocations([FromServices] ILocationService locationService,string? name)
        {
            var locations = await locationService.List(name);
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.LocationMapper.ToDtoList(locations));
        }

        private static async Task<IResult> GetLocation([FromServices] ILocationService locationService, Guid id)
        {
            var location = await locationService.Get(id);
            if (location == null)
                return Results.NotFound();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.LocationMapper.ToDto(location));
        }

        private static async Task<IResult> CreateLocation([FromServices] ILocationService locationService, [FromBody] Dto.Location dtoLocation)
        {
            DomainEntities.Location location = DomainEntities.Location.Create(dtoLocation.Name);

            var createdLocationId = await locationService.Create(location);
            return Results.Created($"/location/Get/{createdLocationId}", new { Id = createdLocationId });
        }

        private static async Task<IResult> UpdateLocation([FromServices] ILocationService locationService, [FromBody] Dto.Location dtoLocation)
        {
            DomainEntities.Location? location = DomainEntities.Location.Reconstitute(dtoLocation.Id, dtoLocation.Name);
            if (location == null)
                return Results.BadRequest();
            await locationService.Update(location);
            return Results.Ok();
        }

        private static async Task<IResult> DeleteLocation([FromServices] ILocationService locationService, Guid id)
        {
            await locationService.Delete(id);
            return Results.Ok();
        }
    }
}
