using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityApplicationLogic.Interfaces
{
    public interface IEmailSender
    {
        Task SendVerificationEmail(string email, string verificationLink);
    }
}
