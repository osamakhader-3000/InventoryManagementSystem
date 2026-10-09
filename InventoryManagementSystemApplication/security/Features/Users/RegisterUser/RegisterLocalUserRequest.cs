using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationLogic.Features.Users.RegisterUser
{
    public record RegisterLocalUserRequest(string UserName, string Email, string Password);
}
