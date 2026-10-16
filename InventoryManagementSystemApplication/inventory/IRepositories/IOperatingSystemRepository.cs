using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces
{
    public interface IOperatingSystemRepository
    {
        Task Create(DomainEntities.OperatingSystem operatingSystem);
        Task<DomainEntities.OperatingSystem?> GetById(Guid id);
        Task<List<DomainEntities.OperatingSystem>> ListAll();
        Task Update(DomainEntities.OperatingSystem operatingSystem);
        Task Delete(Guid id);
    }
}
