using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationLogic.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct);

        // Runs the work in one database transaction: commits if it completes, rolls back if it throws
        Task ExecuteInTransactionAsync(Func<Task> work, CancellationToken ct = default);
    }
}
