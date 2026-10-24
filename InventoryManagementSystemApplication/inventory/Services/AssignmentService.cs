using InventoryManagementSystemApplication.inventory.Exceptions;
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
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly IAccessoryRepository _accessoryRepository;
        private readonly IDeviceRepository _deviceRepository;
        private readonly ITrialLogRepository _trialLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly AssignmentReleaser _assignmentReleaser;
        public AssignmentService(
            IAssignmentRepository assignmentRepository,
            IAccessoryRepository accessoryRepository,
            IDeviceRepository deviceRepository,
            ITrialLogRepository trialLogRepository,
            IUnitOfWork unitOfWork,
            AssignmentReleaser assignmentReleaser)
        {
            _assignmentRepository = assignmentRepository;
            _accessoryRepository = accessoryRepository;
            _deviceRepository = deviceRepository;
            _trialLogRepository = trialLogRepository;
            _unitOfWork = unitOfWork;
            _assignmentReleaser = assignmentReleaser;
        }

        public async Task<long> Create(Assignment assignment, Guid agentId)
        {
            long createdAssignmentId = 0;

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                createdAssignmentId = await _assignmentRepository.Create(assignment);

                await TakeDevice(assignment.Device.Id);

                await IncrementAssignedQuantities(DistinctAccessoryIds(assignment));

                await AddTrialLog(assignment, TransactionType.Assign, agentId);

                await AddAccessoryTrialLogs(assignment, TransactionType.Assign, agentId);
            });

            return createdAssignmentId;
        }

        public async Task Return(long assignmentId, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var existingAssignment = await _assignmentRepository.GetById(assignmentId);
                if (existingAssignment == null)
                    return;

                await _assignmentReleaser.Release(existingAssignment, agentId);
            });
        }

        private async Task TakeDevice(Guid deviceId)
        {
            // Throwing rolls back the whole transaction
            if (!await _deviceRepository.MarkTaken(deviceId))
                throw new DeviceUnavailableException(deviceId);
        }

        private async Task IncrementAssignedQuantities(IEnumerable<Guid> accessoryIds)
        {
            foreach (var accessoryId in accessoryIds)
            {
                // Throwing rolls back the whole transaction
                if (!await _accessoryRepository.IncrementAssignedQuantity(accessoryId))
                    throw new AccessoryOutOfStockException(accessoryId);
            }
        }

        private Task AddTrialLog(Assignment assignment, TransactionType transactionType, Guid agentId)
        {
            return _trialLogRepository.Create(TrialLog.Create(
                assignment.Device.Id,
                assignment.AssignedUser.Id,
                DateTime.UtcNow,
                transactionType,
                agentId));
        }

        // One log per accessory, matching the single unit of stock each one takes
        private async Task AddAccessoryTrialLogs(Assignment assignment, TransactionType transactionType, Guid agentId)
        {
            foreach (var accessory in assignment.Accessories.DistinctBy(x => x.Id))
                await _trialLogRepository.Create(TrialLog.CreateForAccessory(
                    accessory.Name,
                    DateTime.UtcNow,
                    transactionType,
                    agentId));
        }

        private static List<Guid> DistinctAccessoryIds(Assignment assignment)
        {
            return assignment.Accessories.Select(x => x.Id).Distinct().ToList();
        }

        public async Task<List<Assignment>> List(Guid? userId, Guid? depId, Guid? locationId, DateTime? purDate, string? model, string? serialNumber, bool? isReturned)
        {
            return await _assignmentRepository.ListAll(userId, depId, locationId, purDate, model, serialNumber, isReturned);
        }
        public async Task<Assignment?> Get(long assignmentId)
        {
            return await _assignmentRepository.GetById(assignmentId);
        }

        public async Task<IEnumerable<Assignment>> ListByAccessoryId(Guid accessoryId)
        {
            return await _assignmentRepository.ListByAccessoryId(accessoryId);
        }
    }
}
