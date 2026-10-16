using InventoryManagementSystemApplication.inventory.Interfaces;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemApplication.inventory.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<Guid> Create(User user)
        {
            await _userRepository.Create(user);
            return user.Id;
        }
        public async Task<List<User>> List(string? name, Guid? departmentId, Guid? locationId)
        {
            return await _userRepository.List(name, departmentId, locationId);
        }
        public async Task<User?> Get(Guid userId)
        {
            return await _userRepository.GetById(userId);
        }
        public async Task Update(User user)
        {
            await _userRepository.Update(user);
        }
        public async Task Delete(Guid userId)
        {
            await _userRepository.Delete(userId);
        }
    }
}
