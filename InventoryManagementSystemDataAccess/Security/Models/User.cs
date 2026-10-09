using Microsoft.AspNetCore.Identity;
using SecurityApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SecurityApplicationDataAccess.Models
{
    public class User : Microsoft.AspNetCore.Identity.IdentityUser<Guid>
    {
        public bool? LocalAccount { get; set; }
        public byte? AccountStatus { get; set; }
        public byte? EmailVerificationStatus { get; set; }
        public virtual ICollection<UserClaim>?  Claims { get; set; }
        public virtual ICollection<UserLogin>? Logins { get; set; }
        public virtual ICollection<UserToken>? Tokens { get; set; }
        public virtual ICollection<UserRole>? UserRoles { get; set; }
    }
}
