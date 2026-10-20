namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Builds.UpdateBuild
{
    public record UpdateBuildRequest(Guid Id, string? Version, string? Link, IReadOnlyCollection<Guid>? ClientIds);
}
