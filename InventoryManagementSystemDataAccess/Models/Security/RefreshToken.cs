using SecurityApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagementSystemDataAccess.Models.Security
{
    public class RefreshToken
    {
        public Guid Id { get; set; } 

        public Guid UserId { get; set; } 

        public string Hash { get; set; } = string.Empty;

        public DateTime IssuedAt { get; set; } 

        public DateTime ExpiresAt { get; set; } 

        public DateTime? RotatedAt { get; set; }

        public byte Status { get; set; } 

        public Guid FamilyId { get; set; } 
    }

}
