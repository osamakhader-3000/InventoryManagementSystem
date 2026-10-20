namespace InventoryManagementSystem.DeploymentRegistry.DataTransferObject
{
    public class Client
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public IList<Build>? Builds { get; set; }

    }
}
