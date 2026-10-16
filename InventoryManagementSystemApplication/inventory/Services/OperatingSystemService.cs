using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using DomainEntities = InventoryManagementSystemDomain.modules.inventory.Entities;

namespace InventoryManagementSystemApplication.inventory.Services
{
    public class OperatingSystemService : IOperatingSystemService
    {
        private readonly IOperatingSystemRepository _operatingSystemRepository;
        public OperatingSystemService(IOperatingSystemRepository operatingSystemRepository)
        {
            _operatingSystemRepository = operatingSystemRepository;
        }
        public async Task<Guid> Create(DomainEntities.OperatingSystem operatingSystem)
        {
            await _operatingSystemRepository.Create(operatingSystem);
            return operatingSystem.Id;
        }
        public async Task<List<DomainEntities.OperatingSystem>> List()
        {
            return await _operatingSystemRepository.ListAll();
        }
        public async Task<DomainEntities.OperatingSystem?> Get(Guid operatingSystemId)
        {
            return await _operatingSystemRepository.GetById(operatingSystemId);
        }
        public async Task Update(DomainEntities.OperatingSystem operatingSystem)
        {
            await _operatingSystemRepository.Update(operatingSystem);
        }
        public async Task Delete(Guid operatingSystemId)
        {
            await _operatingSystemRepository.Delete(operatingSystemId);
        }
    }
}
