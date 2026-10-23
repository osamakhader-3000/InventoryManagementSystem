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
            ,string? status
            ,Guid? lctnId)
        {
            InventoryManagementSystemDomain.modules.inventory.Enums.TransactionType? transactionType = null;
            if (ta.HasValue)            
                transactionType = (InventoryManagementSystemDomain.modules.inventory.Enums.TransactionType)ta.Value;

            var trialLogs = await trialLogService.List(userid, depid, eventDate, transactionType, sn, model, status, lctnId);
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.TrialLogMapper.ToDtoList(trialLogs));
        }
    }
}
