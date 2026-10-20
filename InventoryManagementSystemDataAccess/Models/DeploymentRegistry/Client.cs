using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace InventoryManagementSystemDataAccess.Models.DeploymentRegistry
{
    public class Client
    {

        [Key]
        public Guid Id { get; set; } 

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }


        public List<Build> Builds { get; set; } = new List<Build>();

    }
}
