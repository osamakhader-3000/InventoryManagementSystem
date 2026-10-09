using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecurityApplicationCore.Models
{
    public class AccessToken(Guid id,string token, DateTime expires_at, string token_type = "Bearer")
    {
        public Guid ID { get; private set; } = id;
        public string Token { get; private set; } = token;
        public DateTime ExpiresAt { get; private set; } = expires_at;
        public string Token_Type { get; private set; } = token_type;
    }
}
