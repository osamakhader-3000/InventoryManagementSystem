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
            deviceGroup.MapPut("/sell", SellDevice).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListDevices([FromServices] IDeviceService deviceService,string? model, string? sn,DateTime? purDate,int? asign)
        {
            if (asign is < byte.MinValue or > byte.MaxValue || asign.HasValue && !Enum.IsDefined(typeof(InventoryManagementSystemDomain.modules.inventory.Enums.AssignedStatus), (byte)asign.Value))
                return Results.BadRequest(new { Message = "Invalid assigned status." });

            var devices = await deviceService.List(model, sn, purDate, asign);
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
                dtoDevice.Status);

            var createdDeviceId = await deviceService.Create(device);
            return Results.Created($"/device/Get/{createdDeviceId}", new { Id = createdDeviceId });
        }

        private static async Task<IResult> UpdateDevice([FromServices] IDeviceService deviceService, [FromBody] Dto.Device dtoDevice)
        {
            DomainEntities.Device? existingDevice = await deviceService.GetBySerialNumber(dtoDevice.SerialNumber);

            
            if(existingDevice != null && existingDevice.Id != dtoDevice.Id)
            {
                return Results.Conflict(new { Message = "A device with the same serial number already exists." });
            }


            DomainEntities.Device? device = DomainEntities.Device.Reconstitute(
                dtoDevice.Id,
                dtoDevice.Model,
                dtoDevice.SerialNumber,
                dtoDevice.Specs,
                dtoDevice.PurchasedDate,
                dtoDevice.RetirementDate,
                dtoDevice.Price,
                dtoDevice.SoldPrice,
                dtoDevice.Status);
            if (device == null)
                return Results.BadRequest();
            await deviceService.Update(device);
            return Results.Ok();
        }

        private static async Task<IResult> DeleteDevice([FromServices] IDeviceService deviceService, Guid id)
        {
            var existingDevice = await deviceService.Get(id);

            if (existingDevice == null)            
                return Results.NotFound();

            if (existingDevice.IsSold == InventoryManagementSystemDomain.modules.inventory.Enums.DeviceSoldState.Sold)
                return Results.Conflict(new { Message = "Device is already sold." });


            if (existingDevice.IsAssigned == InventoryManagementSystemDomain.modules.inventory.Enums.AssignedStatus.Yes)            
                return Results.BadRequest(new { Message = "Cannot delete a taken device." });            
            

            await deviceService.Delete(id);
            return Results.Ok();
        }

        private static async Task<IResult> SellDevice([FromServices] IDeviceService deviceService, [FromBody] Dto.SellDeviceRequest sellRequest)
        {
            if (sellRequest.Price < 0)
                return Results.BadRequest(new { Message = "Sold price cannot be negative." });

            var existingDevice = await deviceService.Get(sellRequest.DeviceId);

            if (existingDevice == null)
                return Results.NotFound();

            if (existingDevice.IsSold == InventoryManagementSystemDomain.modules.inventory.Enums.DeviceSoldState.Sold)
                return Results.Conflict(new { Message = "Device is already sold." });

            if (existingDevice.IsAssigned == InventoryManagementSystemDomain.modules.inventory.Enums.AssignedStatus.Yes)
                return Results.Conflict(new { Message = "Cannot sell a taken device." });

            // The checks above give specific messages; Sell re-checks atomically in case another request changed the device in between
            if (!await deviceService.Sell(sellRequest.DeviceId, sellRequest.Price))
                return Results.Conflict(new { Message = "Device is no longer available for sale." });

            return Results.Ok();
        }

    }
}
