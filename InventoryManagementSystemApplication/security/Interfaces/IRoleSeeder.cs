using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationLogic.Interfaces
{
    public interface IRoleSeeder
    {
        Task SeedAsync(CancellationToken cancellationToken = default);
    }
}
