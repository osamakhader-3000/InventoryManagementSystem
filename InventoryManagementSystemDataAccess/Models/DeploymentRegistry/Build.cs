using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace InventoryManagementSystemDataAccess.Models.DeploymentRegistry
{
    public class Build
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Version { get; set; } = string.Empty;
        public string? Link { get; set; }
        public List<Client> Clients { get; set; } = new List<Client>();

    }
}
