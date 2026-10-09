using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationLogic.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync(CancellationToken ct);
    }
}
