using InventoryManagementSystemApplication.deploymentRegistry.ReadModels;
using Dto = InventoryManagementSystem.DeploymentRegistry.DataTransferObject;

namespace InventoryManagementSystem.DeploymentRegistry.Mappers
{
    public class BuildMapper
    {
        public static Dto.Build ToDto(BuildDetails build)
        {
            return new Dto.Build
            {
                Id = build.Id,
                Version = build.Version,
                Link = build.Link,
                Clients = build.Clients.Select(c => new Dto.Client { Id = c.Id, Name = c.Name }).ToList()
            };
        }

        public static List<Dto.Build> ToDtoList(IEnumerable<BuildDetails> builds)
        {
            return builds.Select(ToDto).ToList();
        }

        public static List<Guid> ToClientIds(Dto.Build build)
        {
            return (build.Clients ?? new List<Dto.Client>())
                .Where(c => c.Id.HasValue)
                .Select(c => c.Id!.Value)
                .ToList();
        }
    }
}
