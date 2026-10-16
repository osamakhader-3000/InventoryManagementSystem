using InventoryManagementSystemApplication.inventory.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Dto = InventoryManagementSystem.Inventory.DataTransferObject;
using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystem.Inventory.APIs
{
    public static class Device
    {
        public static void Map(WebApplication app, RouteGroupBuilder apiGroup)
        {
            RouteGroupBuilder deviceGroup = apiGroup.MapGroup("/device");
            deviceGroup.MapGet("/list", ListDevices).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            deviceGroup.MapGet("/Get/{id}", GetDevice).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            deviceGroup.MapPost("/create", CreateDevice).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            deviceGroup.MapPut("/update", UpdateDevice).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            deviceGroup.MapDelete("/delete/{id}", DeleteDevice).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            deviceGroup.MapPut("/sell/", SellDevice).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListDevices([FromServices] IDeviceService deviceService)
        {
            var devices = await deviceService.List();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.DeviceMapper.ToDtoList(devices));
        }

        private static async Task<IResult> GetDevice([FromServices] IDeviceService deviceService, Guid id)
        {
            var device = await deviceService.Get(id);
            if (device == null)
                return Results.NotFound();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.DeviceMapper.ToDto(device));
        }

        private static async Task<IResult> CreateDevice([FromServices] IDeviceService deviceService, [FromBody] Dto.Device dtoDevice)
        {
            DomainEntities.Device? existingDevice = await deviceService.GetBySerialNumber(dtoDevice.SerialNumber);
            if (existingDevice != null)
            {
                return Results.Conflict(new { Message = "A device with the same serial number already exists." });
            }

            DomainEntities.Device device = DomainEntities.Device.Create(
                dtoDevice.Model,
                dtoDevice.SerialNumber,
                dtoDevice.Specs,
                dtoDevice.PurchasedDate,
                dtoDevice.RetirementDate,
                dtoDevice.Price,
                dtoDevice.SoldPrice,
                (InventoryManagementSystemDomain.modules.inventory.Enums.OperatingSystem)dtoDevice.OperatingSystem,
                (InventoryManagementSystemDomain.modules.inventory.Enums.DeviceStatus)dtoDevice.Status);

            var createdDeviceId = await deviceService.Create(device);
            return Results.Created($"/device/Get/{createdDeviceId}", new { Id = createdDeviceId });
        }

        private static async Task<IResult> UpdateDevice([FromServices] IDeviceService deviceService, [FromBody] Dto.Device dtoDevice)
        {
            DomainEntities.Device? device = DomainEntities.Device.Reconstitute(
                dtoDevice.Id,
                dtoDevice.Model,
                dtoDevice.SerialNumber,
                dtoDevice.Specs,
                dtoDevice.PurchasedDate,
                dtoDevice.RetirementDate,
                dtoDevice.Price,
                dtoDevice.SoldPrice,
                (InventoryManagementSystemDomain.modules.inventory.Enums.OperatingSystem)dtoDevice.OperatingSystem,
                (InventoryManagementSystemDomain.modules.inventory.Enums.DeviceStatus)dtoDevice.Status);
            if (device == null)
                return Results.BadRequest();
            await deviceService.Update(device);
            return Results.Ok();
        }

        private static async Task<IResult> DeleteDevice([FromServices] IDeviceService deviceService, Guid id)
        {
            await deviceService.Delete(id);
            return Results.Ok();
        }

        private static async Task<IResult> SellDevice([FromServices] IDeviceService deviceService, [FromBody] Dto.Device dtoDevice)
        {
            var existingDevice = await deviceService.Get(dtoDevice.Id);

            if(existingDevice == null)
            {
                return Results.NotFound();
            }

            DomainEntities.Device? device = DomainEntities.Device.Reconstitute(
                existingDevice.Id,
                existingDevice.Model,
                existingDevice.SerialNumber,
                existingDevice.Specs,
                existingDevice.PurchasedDate,
                existingDevice.RetirementDate,
                existingDevice.Price,
                dtoDevice.SoldPrice,
                existingDevice.OperatingSystem,
                (InventoryManagementSystemDomain.modules.inventory.Enums.DeviceStatus)dtoDevice.Status);
            if (device == null)
                return Results.BadRequest();
            await deviceService.Update(device);
            return Results.Ok();
        }

    }
}
