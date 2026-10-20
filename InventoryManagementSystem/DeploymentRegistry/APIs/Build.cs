using InventoryManagementSystem.DeploymentRegistry.Mappers;
using InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.CreateBuild;
using InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.DeleteBuild;
using InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.GetBuild;
using InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.ListBuilds;
using InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.UpdateBuild;
using InventoryManagementSystemDomain.modules.deploymentRegistry.Enums.Errors;
using Microsoft.AspNetCore.Mvc;
using Dto = InventoryManagementSystem.DeploymentRegistry.DataTransferObject;

namespace InventoryManagementSystem.DeploymentRegistry.APIs
{
    public static class Build
    {
        public static void Map(WebApplication app, RouteGroupBuilder apiGroup)
        {
            RouteGroupBuilder buildGroup = apiGroup.MapGroup("/build");
            buildGroup.MapGet("/list", ListBuilds).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            buildGroup.MapGet("/Get/{id}", GetBuild).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            buildGroup.MapPost("/create", CreateBuild).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            buildGroup.MapPut("/update", UpdateBuild).RequireAuthorization("VerifiedEmail", "ActiveAccount");
            buildGroup.MapDelete("/delete/{id}", DeleteBuild).RequireAuthorization("VerifiedEmail", "ActiveAccount");
        }

        private static async Task<IResult> ListBuilds([FromServices] ListBuildsUseCase listBuilds, string? version)
        {
            var builds = await listBuilds.Execute(version);
            return Results.Ok(BuildMapper.ToDtoList(builds));
        }

        private static async Task<IResult> GetBuild([FromServices] GetBuildUseCase getBuild, Guid id)
        {
            var build = await getBuild.Execute(id);
            if (build == null)
                return Results.NotFound();
            return Results.Ok(BuildMapper.ToDto(build));
        }

        private static async Task<IResult> CreateBuild([FromServices] CreateBuildUseCase createBuild, [FromBody] Dto.Build dtoBuild)
        {
            var result = await createBuild.Execute(new CreateBuildRequest(dtoBuild.Version, dtoBuild.Link, BuildMapper.ToClientIds(dtoBuild)));
            if (!result.IsSuccess)
                return ToErrorResult(result.Error!.Value, result.Message);

            return Results.Created($"/build/Get/{result.Value}", new { Id = result.Value });
        }

        private static async Task<IResult> UpdateBuild([FromServices] UpdateBuildUseCase updateBuild, [FromBody] Dto.Build dtoBuild)
        {
            if (dtoBuild.Id == null)
                return Results.BadRequest("Build id is required.");

            var result = await updateBuild.Execute(new UpdateBuildRequest(dtoBuild.Id.Value, dtoBuild.Version, dtoBuild.Link, BuildMapper.ToClientIds(dtoBuild)));
            if (!result.IsSuccess)
                return ToErrorResult(result.Error!.Value, result.Message);

            return Results.Ok();
        }

        private static async Task<IResult> DeleteBuild([FromServices] DeleteBuildUseCase deleteBuild, Guid id)
        {
            var result = await deleteBuild.Execute(id);
            if (!result.IsSuccess)
                return ToErrorResult(result.Error!.Value, result.Message);

            return Results.Ok();
        }

        private static IResult ToErrorResult(BuildError error, string? message) =>
            error switch
            {
                BuildError.NotFound => Results.NotFound(message),
                _ => Results.BadRequest(message)
            };
    }
}
