using Microsoft.AspNetCore.Http;
using SecurityApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityApplicationLogic.Interfaces
{
    public interface ICookiesService
    {
        void AppendAccessCookie(Microsoft.AspNetCore.Http.IResponseCookies cookies, SecurityApplicationCore.Models.AccessToken token);
        void AppendRefreshCookie(Microsoft.AspNetCore.Http.IResponseCookies cookies, SecurityApplicationCore.Models.RefreshToken token);
        void AppendUserTokens(IResponseCookies cookies, User user);
    }
}
