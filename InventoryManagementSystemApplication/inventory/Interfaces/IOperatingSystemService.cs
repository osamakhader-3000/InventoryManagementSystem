using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystemApplication.inventory.Interfaces
{
    public interface IOperatingSystemService
    {
        public Task<Guid> Create(DomainEntities.OperatingSystem operatingSystem);

        public Task<List<DomainEntities.OperatingSystem>> List();

        public Task<DomainEntities.OperatingSystem?> Get(Guid operatingSystemId);

        public Task Update(DomainEntities.OperatingSystem operatingSystem);

        public Task Delete(Guid operatingSystemId);
    }
}
