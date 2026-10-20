namespace InventoryManagementSystemApplication.deploymentRegistry.Features.Clients.UpdateClient
{
    public record UpdateClientRequest(Guid Id, string? Name, string? Description);
}
