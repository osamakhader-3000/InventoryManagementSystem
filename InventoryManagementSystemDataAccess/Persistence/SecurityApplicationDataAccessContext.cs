using InventoryManagementSystemDataAccess.Models.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using Inventory = InventoryManagementSystemDataAccess.Models.Inventory;
using DeploymentRegistry = InventoryManagementSystemDataAccess.Models.DeploymentRegistry;

namespace InventoryManagementSystemDataAccess.Persistence
{
    public partial class SecurityApplicationDataAccessContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {

        internal virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        internal virtual DbSet<Inventory.Department> InventoryDepartments { get; set; }

        internal virtual DbSet<Inventory.User> InventoryUsers { get; set; }

        internal virtual DbSet<Inventory.Location> InventoryLocations { get; set; }

        internal virtual DbSet<Inventory.Accessory> InventoryAccessories { get; set; }

        internal virtual DbSet<Inventory.Device> InventoryDevices { get; set; }

        internal virtual DbSet<Inventory.Assignment> InventoryAssignments { get; set; }

        internal virtual DbSet<Inventory.TrialLog> InventoryTrialLogs { get; set; }

        internal virtual DbSet<Inventory.AssignmentAccessory> InventoryAssignmentAccessories { get; set; }

        internal virtual DbSet<Inventory.OperatingSystem> InventoryOperatingSystems { get; set; }

        internal virtual DbSet<DeploymentRegistry.Build> DeploymentRegistryBuilds { get; set; }

        internal virtual DbSet<DeploymentRegistry.Client> DeploymentRegistryClients { get; set; }


        //internal virtual DbSet<SecurityApplicationDataAccess.Models.AccessToken> AccessTokens { get; set; }

        public SecurityApplicationDataAccessContext(DbContextOptions<SecurityApplicationDataAccessContext> options)
        : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Role>().ToTable("roles");
            modelBuilder.Entity<UserClaim>().ToTable("user_claims");
            modelBuilder.Entity<UserRole>().ToTable("user_roles");
            modelBuilder.Entity<UserLogin>().ToTable("user_logins");
            modelBuilder.Entity<UserToken>().ToTable("user_tokens");
            modelBuilder.Entity<RoleClaim>().ToTable("role_claims");
            modelBuilder.Entity<IdentityUserPasskey<Guid>>().ToTable("user_passkeys");

            modelBuilder.Entity<Inventory.Department>().ToTable("inventory_departments");
            modelBuilder.Entity<Inventory.User>().ToTable("inventory_users");
            modelBuilder.Entity<Inventory.Location>().ToTable("inventory_locations");
            modelBuilder.Entity<Inventory.Accessory>().ToTable("inventory_accessories");
            modelBuilder.Entity<Inventory.Device>().ToTable("inventory_devices");
            modelBuilder.Entity<Inventory.Assignment>().ToTable("inventory_assignments");
            modelBuilder.Entity<Inventory.TrialLog>().ToTable("inventory_trial_logs");
            modelBuilder.Entity<Inventory.AssignmentAccessory>().ToTable("inventory_assignment_accessories");
            modelBuilder.Entity<Inventory.OperatingSystem>().ToTable("inventory_operating_systems");

            modelBuilder.Entity<Inventory.Device>(b =>
            {
                b.Property(e => e.Price).HasPrecision(12, 2);
                b.Property(e => e.SoldPrice).HasPrecision(12, 2);

                b.HasIndex(e => e.SerialNumber).IsUnique();

                // Many-to-one: a device has one operating system, an operating system has many devices
                b.HasOne(e => e.OperatingSystem)
                    .WithMany(e => e.Devices)
                    .HasForeignKey(e => e.OperatingSystemId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Many-to-one: a user has one department, a department has many users
            modelBuilder.Entity<Inventory.User>(b =>
            {
                b.HasOne(e => e.Department)
                    .WithMany(e => e.Users)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Optional: a user may have no location
                b.HasOne(e => e.Location)
                    .WithMany(e => e.Users)
                    .HasForeignKey(e => e.LocationId)
                    .IsRequired(false)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Many-to-one: an assignment has one user and one device
            modelBuilder.Entity<Inventory.Assignment>(b =>
            {
                b.HasOne(e => e.AssignedUser)
                    .WithMany()
                    .HasForeignKey(e => e.AssignedUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(e => e.Device)
                    .WithMany()
                    .HasForeignKey(e => e.DeviceId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Many-to-one: a trial log has one user, one device and one agent (security user)
            modelBuilder.Entity<Inventory.TrialLog>(b =>
            {
                b.HasOne(e => e.User)
                    .WithMany(e => e.TrialLogs)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(e => e.Device)
                    .WithMany(e => e.TrialLogs)
                    .HasForeignKey(e => e.DeviceId)
                    .OnDelete(DeleteBehavior.Restrict);

                b.HasOne(e => e.Agent)
                    .WithMany()
                    .HasForeignKey(e => e.AgentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Many-to-many: an assignment has many accessories, an accessory belongs to many assignments
            modelBuilder.Entity<Inventory.AssignmentAccessory>(b =>
            {
                b.HasKey(e => new { e.AssignmentId, e.AccessoryId });

                b.HasOne(e => e.Assignment)
                    .WithMany(e => e.AssignmentAccessories)
                    .HasForeignKey(e => e.AssignmentId)
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne(e => e.Accessory)
                    .WithMany(e => e.AssignmentAccessories)
                    .HasForeignKey(e => e.AccessoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DeploymentRegistry.Build>().ToTable("deployment_registry_builds");
            modelBuilder.Entity<DeploymentRegistry.Client>().ToTable("deployment_registry_clients");

            // Many-to-many: a build is deployed to many clients, a client runs many builds.
            // A build cannot be deleted while related to clients; deleting a client removes it from its builds.
            modelBuilder.Entity<DeploymentRegistry.Build>(b =>
            {
                b.HasIndex(e => e.Version);

                b.HasMany(e => e.Clients)
                    .WithMany(e => e.Builds)
                    .UsingEntity<Dictionary<string, object>>(
                        "deployment_registry_build_clients",
                        r => r.HasOne<DeploymentRegistry.Client>().WithMany().HasForeignKey("ClientId").OnDelete(DeleteBehavior.Cascade),
                        l => l.HasOne<DeploymentRegistry.Build>().WithMany().HasForeignKey("BuildId").OnDelete(DeleteBehavior.Restrict),
                        j => j.HasKey("BuildId", "ClientId"));
            });

            modelBuilder.Entity<DeploymentRegistry.Client>(b =>
            {
                b.HasIndex(e => e.Name);
            });



            modelBuilder.Entity<User>(b =>
            {
                b.HasMany(e => e.Claims)
                    .WithOne(e => e.User)
                    .HasForeignKey(uc => uc.UserId)
                    .IsRequired();

                b.HasMany(e => e.Logins)
                    .WithOne(e => e.User)
                    .HasForeignKey(ul => ul.UserId)
                    .IsRequired();

                b.HasMany(e => e.Tokens)
                    .WithOne(e => e.User)
                    .HasForeignKey(ut => ut.UserId)
                    .IsRequired();

                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            modelBuilder.Entity<Role>(b =>
            {
                b.HasMany(e => e.UserRoles)
                    .WithOne(e => e.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                b.HasMany(e => e.RoleClaims)
                    .WithOne(e => e.Role)
                    .HasForeignKey(rc => rc.RoleId)
                    .IsRequired();
            });

            modelBuilder.Entity<Role>().HasIndex(r => r.NormalizedName).HasDatabaseName("ix_roles_normalized_name");

            modelBuilder.Entity<User>().HasIndex(u => u.NormalizedEmail).HasDatabaseName("ix_users_normalized_email");

            modelBuilder.Entity<User>().HasIndex(u => u.NormalizedUserName).HasDatabaseName("ix_users_normalized_user_name").IsUnique();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);


    }

}