using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }
        public async Task<Guid> Create(Location location)
        {
            await _locationRepository.Create(location);
            return location.Id;
        }
        public async Task<List<Location>> List(string? name)
        {
            return await _locationRepository.ListAll(name);
        }
        public async Task<Location?> Get(Guid locationId)
        {
            return await _locationRepository.GetById(locationId);
        }
        public async Task Update(Location location)
        {
            await _locationRepository.Update(location);
        }
        public async Task Delete(Guid locationId)
        {
            await _locationRepository.Delete(locationId);
        }
    }
}
