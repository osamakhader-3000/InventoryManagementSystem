namespace InventoryManagementSystem.DeploymentRegistry.DataTransferObject
{
    public class Build
    {
        public Guid? Id { get; set; }
        public string? Version { get; set; }
        public string? Link { get; set; }
        public List<Client>? Clients { get; set; }

    }
}
