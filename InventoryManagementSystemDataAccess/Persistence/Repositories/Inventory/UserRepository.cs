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
            Invent.User? dbUser = await _context.InventoryUsers.Include(x => x.Department).FirstOrDefaultAsync(x => x.Id == id);

            if (dbUser == null)
                return null;

            return dbUser.ToDomainModel();
        }

        public async Task<List<User>> ListAll()
        {
            var dbUsers = await _context.InventoryUsers.Include(x => x.Department).ToListAsync();
            return dbUsers.ToDomainModelList(x => x.ToDomainModel());
        }

        public Task Update(User user)
        {
            _context.InventoryUsers.Update(user.ToDataAccessModel());
            return _context.SaveChangesAsync();
        }
    }
}
