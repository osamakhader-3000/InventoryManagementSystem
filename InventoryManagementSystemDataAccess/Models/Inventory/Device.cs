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

        public byte OperatingSystem { get; set; }

        public string Specs { get; set; } = string.Empty;
              

        public DateTime? PurchasedDate { get; set; }

        public DateTime? RetirementDate { get; set; }

        public double Price { get; set; }

        public double SoldPrice { get; set; }

    }
}
