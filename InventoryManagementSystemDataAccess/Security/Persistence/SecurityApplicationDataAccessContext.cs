using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SecurityApplicationDataAccess.Models;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace SecurityApplicationDataAccess.Persistence
{
    public partial class SecurityApplicationDataAccessContext : IdentityDbContext<User, Role, Guid, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {

        internal virtual DbSet<RefreshToken> RefreshTokens { get; set; }

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