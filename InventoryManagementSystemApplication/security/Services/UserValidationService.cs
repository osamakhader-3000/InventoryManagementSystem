using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationLogic.Services
{
    public class UserValidationService : Interfaces.IUserValidationService
    {
        public bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (password.Length < 8 || password.Length > 50)
                return false;

            return true;
        }


        public bool IsUserNameValid(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;
            if (username.Length < 5 || username.Length > 50)
                return false;
            foreach (char c in username)
            {
                if (!char.IsLetterOrDigit(c) && c != '_' && c != '-' && c != '.')
                    return false;
            }
            return true;
        }
    }
}
