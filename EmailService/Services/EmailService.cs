using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace EmailService.Services
{
    public class EmailSettings
    {
        public string Name { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; }
        public bool UseSsl { get;  set; } = false;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
