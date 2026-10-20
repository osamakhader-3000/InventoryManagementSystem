using InventoryManagementSystemApplication.deploymentRegistry.ReadModels;
using Dto = InventoryManagementSystem.DeploymentRegistry.DataTransferObject;

namespace InventoryManagementSystem.DeploymentRegistry.Mappers
{
    public class ClientMapper
    {
        public static Dto.Client ToDto(ClientDetails client)
        {
            return new Dto.Client
            {
                Id = client.Id,
                Name = client.Name,
                Description = client.Description,
                Builds = client.Builds.Select(b => new Dto.Build { Id = b.Id, Version = b.Version }).ToList()
            };
        }

        public static List<Dto.Client> ToDtoList(IEnumerable<ClientDetails> clients)
        {
            return clients.Select(ToDto).ToList();
        }
    }
}
