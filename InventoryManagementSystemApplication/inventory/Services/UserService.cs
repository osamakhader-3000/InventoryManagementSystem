using InventoryManagementSystemApplication.inventory.Exceptions;
using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.Enums;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using IUnitOfWork = SecurityApplicationLogic.Interfaces.IUnitOfWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly AssignmentReleaser _assignmentReleaser;
        private readonly ITrialLogRepository _trialLogRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UserService(
            IUserRepository userRepository,
            IAssignmentRepository assignmentRepository,
            AssignmentReleaser assignmentReleaser,
            ITrialLogRepository trialLogRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _assignmentRepository = assignmentRepository;
            _assignmentReleaser = assignmentReleaser;
            _trialLogRepository = trialLogRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Create(User user, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await _userRepository.Create(user);
                await AddTrialLog(user.Id, TransactionType.Create, agentId);
            });
            return user.Id;
        }
        public async Task<List<User>> List(string? name, Guid? departmentId, Guid? locationId, UserSuspensionState? status)
        {
            return await _userRepository.List(name, departmentId, locationId, status);
        }
        public async Task<User?> Get(Guid userId)
        {
            return await _userRepository.GetById(userId);
        }
        public async Task Update(User user, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Log after the update: the log copies the user's new name and location
                await _userRepository.Update(user);
                await AddTrialLog(user.Id, TransactionType.Update, agentId);
            });
        }
        public async Task Suspend(Guid userId, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                // Throwing rolls back the whole transaction, including any assignments already released
                if (!await _userRepository.Suspend(userId))
                    throw new UserAlreadySuspendedException(userId);

                await AddTrialLog(userId, TransactionType.Suspend, agentId);

                var openAssignments = await _assignmentRepository.ListAll(userId, null, null, null, null, null, false);
                foreach (var assignment in openAssignments)
                    await _assignmentReleaser.Release(assignment, agentId);
            });
        }
        public async Task Unsuspend(Guid userId, Guid agentId)
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (!await _userRepository.Unsuspend(userId))
                    throw new UserNotSuspendedException(userId);

                await AddTrialLog(userId, TransactionType.Activate, agentId);
            });
        }

        private Task AddTrialLog(Guid userId, TransactionType transactionType, Guid agentId)
        {
            return _trialLogRepository.Create(TrialLog.Create(
                null,
                userId,
                DateTime.UtcNow,
                transactionType,
                agentId,
                LogEntityType.User));
        }
    }
}
