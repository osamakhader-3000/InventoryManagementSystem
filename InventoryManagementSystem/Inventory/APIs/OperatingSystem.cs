using InventoryManagementSystemApplication.inventory.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Dto = InventoryManagementSystem.Inventory.DataTransferObject;
using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystem.Inventory.APIs
{
    public static class OperatingSystem
    {
        public static void Map(WebApplication app, RouteGroupBuilder apiGroup)
        {
            RouteGroupBuilder operatingSystemGroup = apiGroup.MapGroup("/operatingsystem");
            operatingSystemGroup.MapGet("/list", ListOperatingSystems).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            operatingSystemGroup.MapGet("/Get/{id}", GetOperatingSystem).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            operatingSystemGroup.MapPost("/create", CreateOperatingSystem).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            operatingSystemGroup.MapPut("/update", UpdateOperatingSystem).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            operatingSystemGroup.MapDelete("/delete/{id}", DeleteOperatingSystem).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListOperatingSystems([FromServices] IOperatingSystemService operatingSystemService)
        {
            var operatingSystems = await operatingSystemService.List();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.OperatingSystemMapper.ToDtoList(operatingSystems));
        }

        private static async Task<IResult> GetOperatingSystem([FromServices] IOperatingSystemService operatingSystemService, Guid id)
        {
            var operatingSystem = await operatingSystemService.Get(id);
            if (operatingSystem == null)
                return Results.NotFound();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.OperatingSystemMapper.ToDto(operatingSystem));
        }

        private static async Task<IResult> CreateOperatingSystem([FromServices] IOperatingSystemService operatingSystemService, [FromBody] Dto.OperatingSystem dtoOperatingSystem)
        {
            List<DomainEntities.OperatingSystem> operatingSystems = await operatingSystemService.List();

            if(operatingSystems.Any(os => os.Name.Equals(dtoOperatingSystem.Name, StringComparison.OrdinalIgnoreCase)))            
                return Results.Conflict($"An operating system with the name '{dtoOperatingSystem.Name}' already exists.");            

            DomainEntities.OperatingSystem operatingSystem = DomainEntities.OperatingSystem.Create(dtoOperatingSystem.Name);

            var createdOperatingSystemId = await operatingSystemService.Create(operatingSystem);
            return Results.Created($"/operatingsystem/Get/{createdOperatingSystemId}", new { Id = createdOperatingSystemId });
        }

        private static async Task<IResult> UpdateOperatingSystem([FromServices] IOperatingSystemService operatingSystemService, [FromBody] Dto.OperatingSystem dtoOperatingSystem)
        {
            List<DomainEntities.OperatingSystem> operatingSystems = await operatingSystemService.List();

            if(operatingSystems.Any(os => os.Name.Equals(dtoOperatingSystem.Name, StringComparison.OrdinalIgnoreCase) && os.Id != dtoOperatingSystem.Id))
                return Results.Conflict($"An operating system with the name '{dtoOperatingSystem.Name}' already exists.");

            DomainEntities.OperatingSystem operatingSystem = DomainEntities.OperatingSystem.Reconstitute(dtoOperatingSystem.Id, dtoOperatingSystem.Name);
            await operatingSystemService.Update(operatingSystem);
            return Results.Ok();
        }

        private static async Task<IResult> DeleteOperatingSystem([FromServices] IOperatingSystemService operatingSystemService, [FromServices] IDeviceService deviceService, Guid id)
        {
            var operatingSystem = await operatingSystemService.Get(id);
            if (operatingSystem == null)
                return Results.NotFound();

            var devicesWithOperatingSystem = await deviceService.List(null, null, operatingSystem.Name, null);

            if (devicesWithOperatingSystem.Any())
                return Results.Conflict($"Cannot delete operating system '{operatingSystem.Name}' because it is used by one or more devices.");

            await operatingSystemService.Delete(id);
            return Results.Ok();
        }
    }
}
