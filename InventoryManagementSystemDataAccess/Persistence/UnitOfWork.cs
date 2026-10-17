using Microsoft.EntityFrameworkCore;
using SecurityApplicationLogic.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SecurityApplicationDataAccessContext _context;
        public UnitOfWork(SecurityApplicationDataAccessContext context)
        {
            _context = context;
        }

        public Task<int> SaveChangesAsync(CancellationToken ct)
        {
            return _context.SaveChangesAsync(ct);
        }

        public async Task ExecuteInTransactionAsync(Func<Task> work, CancellationToken ct = default)
        {
            // Repositories share this scoped context, so their SaveChangesAsync calls join the open transaction.
            // The execution strategy keeps this valid if EnableRetryOnFailure is turned on later.
            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    await work();
                    await transaction.CommitAsync(ct);
                }
                catch
                {
                    await transaction.RollbackAsync(ct);
                    // Drop tracked entities whose changes were rolled back
                    _context.ChangeTracker.Clear();
                    throw;
                }
            });
        }
    }
}
