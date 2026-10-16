using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        public DeviceService(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }
        public async Task<Guid> Create(Device device)
        {
            await _deviceRepository.Create(device);
            return device.Id;
        }
        public async Task<List<Device>> List(string? model,string? serialNumber,string? operatingSystem,DateTime? purchasedDate)
        {
            return await _deviceRepository.List(model, serialNumber, operatingSystem, purchasedDate);
        }
        public async Task<Device?> Get(Guid deviceId)
        {
            return await _deviceRepository.GetById(deviceId);
        }
        public async Task<Device?> GetBySerialNumber(string serialNumber)
        {
            return await _deviceRepository.GetBySerialNumber(serialNumber);
        }
        public async Task Update(Device device)
        {
            await _deviceRepository.Update(device);
        }
        public async Task Delete(Guid deviceId)
        {
            await _deviceRepository.Delete(deviceId);
        }
    }
}
