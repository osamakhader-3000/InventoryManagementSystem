using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityApplicationLogic.Interfaces
{
    public interface IExternalLoginService
    {
        Task<SecurityApplicationCore.Models.User> FindOrProvisionUserFromExternalAsync(System.Security.Claims.ClaimsPrincipal externalPrincipal);


    }
}
