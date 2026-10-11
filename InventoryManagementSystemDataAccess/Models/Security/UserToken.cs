using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Models.Security
{
    public class UserToken : IdentityUserToken<Guid>
    {
        public virtual User? User { get; set; }
    }
}
