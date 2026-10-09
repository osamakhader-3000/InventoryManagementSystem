using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationDataAccess.Models
{
    public class UserClaim : IdentityUserClaim<Guid>
    {
        public virtual User? User { get; set; }

    }
}
