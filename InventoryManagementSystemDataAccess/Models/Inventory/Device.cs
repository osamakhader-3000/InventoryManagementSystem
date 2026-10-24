using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryManagementSystemDataAccess.Models.Inventory
{
    public class Device
    {
        public Guid Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;

        public string Specs { get; set; } = string.Empty;
              

        public DateTime? PurchasedDate { get; set; }

        public DateTime? RetirementDate { get; set; }

        public decimal Price { get; set; }

        public decimal? SoldPrice { get; set; }

        public string? Status { get; set; }

        public byte IsAssigned { get; set; }

        public byte IsSold { get; set; } = 0;

    }
}
