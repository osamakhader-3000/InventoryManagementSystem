namespace InventoryManagementSystemApplication.deploymentRegistry.ReadModels
{
    public record BuildSummary(Guid Id, string Version);

    public record BuildDetails(Guid Id, string Version, string? Link, IReadOnlyList<ClientSummary> Clients);
}
