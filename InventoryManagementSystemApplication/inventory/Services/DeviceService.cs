using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.Enums;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using SecurityApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly ITrialLogRepository _trialLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DeviceService(
            IDeviceRepository deviceRepository,
            ITrialLogRepository trialLogRepository,
            IUnitOfWork unitOfWork)
        {
            _deviceRepository = deviceRepository;
            _trialLogRepository = trialLogRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Create(Device device, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _deviceRepository.Create(device);
                await AddTrialLog(device.Id, TransactionType.Create, agentId);
            });
            return device.Id;
        }
        public async Task<List<Device>> List(string? model,string? serialNumber,DateTime? purchasedDate,int? isAssigned)
        {
            return await _deviceRepository.List(model, serialNumber, purchasedDate, isAssigned);
        }
        public async Task<Device?> Get(Guid deviceId)
        {
            return await _deviceRepository.GetById(deviceId);
        }
        public async Task<Device?> GetBySerialNumber(string serialNumber)
        {
            return await _deviceRepository.GetBySerialNumber(serialNumber);
        }
        public async Task Update(Device device, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _deviceRepository.Update(device);
                await AddTrialLog(device.Id, TransactionType.Update, agentId);
            });
        }
        public async Task Delete(Guid deviceId, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Log first: the log copies the device's details, which are gone after the delete
                await AddTrialLog(deviceId, TransactionType.Delete, agentId);
                await _deviceRepository.Delete(deviceId);
            });
        }
        public async Task<bool> Sell(Guid deviceId, decimal soldPrice, Guid agentId)
        {
            bool sold = false;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                sold = await _deviceRepository.MarkSold(deviceId, soldPrice);

                // Only log a sale that happened; a lost race leaves nothing to log
                if (sold)
                    await AddTrialLog(deviceId, TransactionType.Sell, agentId);
            });

            return sold;
        }

        private Task AddTrialLog(Guid deviceId, TransactionType transactionType, Guid agentId)
        {
            return _trialLogRepository.Create(TrialLog.Create(
                deviceId,
                null,
                DateTime.UtcNow,
                transactionType,
                agentId,
                LogEntityType.Device));
        }
    }
}
