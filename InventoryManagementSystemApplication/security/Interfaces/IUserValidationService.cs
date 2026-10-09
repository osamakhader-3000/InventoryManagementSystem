using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationLogic.Interfaces
{
    public interface IUserValidationService
    {
        bool IsPasswordValid(string password);
        bool IsUserNameValid(string username);
    }
}
