namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.CreateBuild
{
    public record CreateBuildRequest(string? Version, string? Link, IReadOnlyCollection<Guid>? ClientIds);
}
