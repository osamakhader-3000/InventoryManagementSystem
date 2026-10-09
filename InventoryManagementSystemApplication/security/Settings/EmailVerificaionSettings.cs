using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityApplicationLogic.Settings
{
    public class EmailVerificaionSettings
    {
        public string VerificationUrl { get; set; } = string.Empty;
        public string EmailVerificationTokenSecretKey { get; set; } = string.Empty;
    }
}
