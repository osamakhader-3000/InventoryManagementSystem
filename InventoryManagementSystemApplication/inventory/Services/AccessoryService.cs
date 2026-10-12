using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Services
{
    public class AccessoryService : IAccessoryService
    {
        private readonly IAccessoryRepository _accessoryRepository;
        public AccessoryService(IAccessoryRepository accessoryRepository)
        {
            _accessoryRepository = accessoryRepository;
        }
        public async Task<Guid> Create(Accessory accessory)
        {
            await _accessoryRepository.Create(accessory);
            return accessory.Id;
        }
        public async Task<List<Accessory>> List()
        {
            return await _accessoryRepository.ListAll();
        }

     
        public async Task<IEnumerable<Accessory>> ListByIds(List<Guid> ids)
        {
            return await _accessoryRepository.ListByIds(ids);
        }

        public async Task<Accessory?> Get(Guid accessoryId)
        {
            return await _accessoryRepository.GetById(accessoryId);
        }
        public async Task Update(Accessory accessory)
        {
            await _accessoryRepository.Update(accessory);
        }
        public async Task Delete(Guid accessoryId)
        {
            await _accessoryRepository.Delete(accessoryId);
        }
    }
}
