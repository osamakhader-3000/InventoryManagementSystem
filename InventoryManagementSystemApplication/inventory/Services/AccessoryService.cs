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
    public class AccessoryService : IAccessoryService
    {
        private readonly IAccessoryRepository _accessoryRepository;
        private readonly ITrialLogRepository _trialLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AccessoryService(
            IAccessoryRepository accessoryRepository,
            ITrialLogRepository trialLogRepository,
            IUnitOfWork unitOfWork)
        {
            _accessoryRepository = accessoryRepository;
            _trialLogRepository = trialLogRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Create(Accessory accessory, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _accessoryRepository.Create(accessory);
                await AddTrialLog(accessory.Name, TransactionType.Create, agentId);
            });
            return accessory.Id;
        }
        public async Task<List<Accessory>> List(string? name,bool? stock, DateTime? date)
        {
            return await _accessoryRepository.List(name,stock,date);
        }

     
        public async Task<IEnumerable<Accessory>> ListByIds(List<Guid> ids)
        {
            return await _accessoryRepository.ListByIds(ids);
        }

        public async Task<Accessory?> Get(Guid accessoryId)
        {
            return await _accessoryRepository.GetById(accessoryId);
        }
        public async Task Update(Accessory accessory, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Log the new name: the log keeps the accessory as it is after the update
                await _accessoryRepository.Update(accessory);
                await AddTrialLog(accessory.Name, TransactionType.Update, agentId);
            });
        }
        public async Task Delete(Guid accessoryId, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Read the name first: it is gone after the delete
                var accessory = await _accessoryRepository.GetById(accessoryId);
                if (accessory == null)
                    return;

                await AddTrialLog(accessory.Name, TransactionType.Delete, agentId);
                await _accessoryRepository.Delete(accessoryId);
            });
        }

        private Task AddTrialLog(string accessoryName, TransactionType transactionType, Guid agentId)
        {
            return _trialLogRepository.Create(TrialLog.CreateForAccessory(
                accessoryName,
                DateTime.UtcNow,
                transactionType,
                agentId));
        }
    }
}
