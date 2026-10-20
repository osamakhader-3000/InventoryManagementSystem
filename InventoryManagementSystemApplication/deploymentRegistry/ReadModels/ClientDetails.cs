namespace InventoryManagementSystemApplication.deploymentRegistry.ReadModels
{
    public record ClientSummary(Guid Id, string Name);

    public record ClientDetails(Guid Id, string Name, string? Description, IReadOnlyList<BuildSummary> Builds);
}
