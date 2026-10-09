using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationDataAccess.Models
{
    public class RoleClaim : IdentityRoleClaim<Guid>
    {
        public virtual Role? Role { get; set; }

    }
}
