using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityApplicationCore.Models
{
    public static class Roles
    {
        public const string Employee = "Employee";
        public const string TechnicalSupport = "TechnicalSupport";
        public const string Admin = "Admin";

        public static readonly IReadOnlyList<string> All = [Employee, TechnicalSupport, Admin];
    }
}
