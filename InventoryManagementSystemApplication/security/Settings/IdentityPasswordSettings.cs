using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationLogic.Settings
{
    public class IdentityPasswordSettings
    {
        public int RequiredLength { get; set; } = 8;
        public bool RequireDigit { get; set; } = false;
        public bool RequireLowercase { get; set; } = false;
        public bool RequireUppercase { get; set; } = false;
        public bool RequireNonAlphanumeric { get; set; } = false;
        public int RequiredUniqueChars { get; set; } = 1;
    }
}
