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

        private static async Task<IResult> ListTrialLogs([FromServices] ITrialLogService trialLogService)
        {
            var trialLogs = await trialLogService.List();
            return Results.Ok(InventoryManagementSystem.Inventory.Mappers.TrialLogMapper.ToDtoList(trialLogs));
        }
    }
}
