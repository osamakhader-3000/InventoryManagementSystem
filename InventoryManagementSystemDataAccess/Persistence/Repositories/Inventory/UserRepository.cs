using InventoryManagementSystemDataAccess.Mappers.Inventory;
using InventoryManagementSystemDomain.modules.inventory.Entities;
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

        public Task Delete(Guid id)
        {
            _context.InventoryUsers.Remove(new Invent.User { Id = id });
            return _context.SaveChangesAsync();
        }

        public async Task<User?> GetById(Guid id)
        {
            Invent.User? dbUser = await UsersWithDepartment().FirstOrDefaultAsync(x => x.Id == id);

            if (dbUser == null)
                return null;

            return dbUser.ToDomainModel();
        }

        public async Task<List<User>> ListAll()
        {
            var dbUsers = await UsersWithDepartment().ToListAsync();
            return dbUsers.ToDomainModelList(x => x.ToDomainModel());
        }

        public async Task Update(User user)
        {
            Invent.User? dbUser = await _context.InventoryUsers.FirstOrDefaultAsync(x => x.Id == user.Id);

            if (dbUser == null)
                return;

            dbUser.Name = user.Name;
            dbUser.DepartmentId = user.Department.Id;
            dbUser.LocationId = user.Location?.Id;

            await _context.SaveChangesAsync();
        }
    }
}
