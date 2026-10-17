using InventoryManagementSystemApplication.inventory.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Inventory.APIs
{
    public static class TrialLog
    {
        public static void Map(WebApplication app, RouteGroupBuilder apiGroup)
        {
            RouteGroupBuilder trialLogGroup = apiGroup.MapGroup("/triallog");
            trialLogGroup.MapGet("/list", ListTrialLogs).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListTrialLogs([FromServices] ITrialLogService trialLogService
            , Guid? userid
            , Guid? depid
            , DateTime? eventDate
            , int? ta
            , string? sn
            , string? model
            ,int? status
            ,Guid? lctnId)
        {
            InventoryManagementSystemDomain.modules.inventory.Enums.TransactionType? transactionType = null;
            if (ta.HasValue)            
                transactionType = (InventoryManagementSystemDomain.modules.inventory.Enums.TransactionType)ta.Value;

            InventoryManagementSystemDomain.modules.inventory.Enums.DeviceStatus? deviceStatus = null;
            if (status.HasValue)
                deviceStatus = (InventoryManagementSystemDomain.modules.inventory.Enums.DeviceStatus)status.Value;

            var trialLogs = await trialLogService.List(userid, depid, eventDate, transactionType, sn, model, deviceStatus, lctnId);
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.TrialLogMapper.ToDtoList(trialLogs));
        }
    }
}
