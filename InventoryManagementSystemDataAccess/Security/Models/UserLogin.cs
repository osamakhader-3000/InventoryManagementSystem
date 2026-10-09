using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationDataAccess.Models
{
    public class UserLogin : IdentityUserLogin<Guid>
    {
        public virtual User? User { get; set; }
    }
}
