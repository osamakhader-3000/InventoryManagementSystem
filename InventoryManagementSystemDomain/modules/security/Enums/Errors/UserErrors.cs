using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityDomain.Enums.Errors
{
    public enum UserError
    {
        EmailAlreadyExists,
        UsernameAlreadyExists,
        InvalidUsernameFormat,
        InvalidPasswordFormat,
        InvalidEmailFormat,
        UnexpectedError
    }
}
