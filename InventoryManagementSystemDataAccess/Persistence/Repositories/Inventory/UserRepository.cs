using InventoryManagementSystemDataAccess.Mappers.Inventory;
using InventoryManagementSystemDataAccess.Persistence.Extensions;
using InventoryManagementSystemDomain.modules.inventory.Entities;
using InventoryManagementSystemDomain.modules.inventory.Enums;
using InventoryManagementSystemDomain.modules.inventory.RepositoriesInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Invent = InventoryManagementSystemDataAccess.Models.Inventory;

namespace InventoryManagementSystemDataAccess.Persistence.Repositories.Inventory
{
    public class UserRepository : IUserRepository
    {
        private readonly SecurityApplicationDataAccessContext _context;
        public UserRepository(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }

        private IQueryable<Invent.User> UsersWithDepartment()
        {
            return _context.InventoryUsers
                .Include(x => x.Department)
                .Include(x => x.Location);
        }

        public Task Create(User user)
        {
            _context.InventoryUsers.Add(user.ToDataAccessModel());
            return _context.SaveChangesAsync();
        }

        public Task<bool> Suspend(Guid id)
        {
            return SetSuspensionState(id, UserSuspensionState.Active, UserSuspensionState.Suspended);
        }

        public Task<bool> Unsuspend(Guid id)
        {
            return SetSuspensionState(id, UserSuspensionState.Suspended, UserSuspensionState.Active);
        }

        // Conditional update: returns false when the user is missing or not in the expected state, so concurrent calls can't both succeed
        private async Task<bool> SetSuspensionState(Guid id, UserSuspensionState from, UserSuspensionState to)
        {
            int affectedRows = await _context.InventoryUsers
                .Where(x => x.Id == id && x.IsSuspended == (byte)from)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsSuspended, (byte)to));
            return affectedRows == 1;
        }

        public async Task<User?> GetById(Guid id)
        {
            Invent.User? dbUser = await UsersWithDepartment().FirstOrDefaultAsync(x => x.Id == id);

            if (dbUser == null)
                return null;

            return dbUser.ToDomainModel();
        }

        public async Task<List<User>> List(string? name, Guid? departmentId, Guid? locationId, UserSuspensionState? status)
        {
            var query = UsersWithDepartment();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.WhereContainsIgnoreCase(x => x.Name, name);
            }

            if (departmentId.HasValue)
            {
                query = query.Where(x => x.DepartmentId == departmentId.Value);
            }

            if (locationId.HasValue)
            {
                query = query.Where(x => x.LocationId == locationId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(x => x.IsSuspended == (byte)status.Value);
            }

            var dbUsers = await query.ToListAsync();
            return dbUsers.ToDomainModelList(x => x.ToDomainModel());
        }

        public async Task Update(User user)
        {
            Invent.User? dbUser = await _context.InventoryUsers.FirstOrDefaultAsync(x => x.Id == user.Id);

            if (dbUser == null)
                return;

            dbUser.Name = user.Name;
            dbUser.DepartmentId = user.Department.Id;
            dbUser.LocationId = user.Location.Id;

            await _context.SaveChangesAsync();
        }
    }
}
