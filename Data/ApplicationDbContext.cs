using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // =========================================================
        // DATABASE TABLES
        // =========================================================

        public DbSet<User> Users { get; set; } = null!;

        public DbSet<Role> Roles { get; set; } = null!;

        public DbSet<InventoryManagementSystem.Models.Permission>
            Permissions { get; set; } = null!;

        public DbSet<RolePermission> RolePermissions { get; set; } = null!;

        public DbSet<Assest> Assets { get; set; } = null!;

        public DbSet<Category> Categories { get; set; } = null!;

        public DbSet<Notification> Notifications { get; set; } = null!;

        public DbSet<AuditLogEntry> AuditLogs { get; set; } = null!;

        public DbSet<Organisation> Organisations { get; set; } = null!;


        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =====================================================
            // USER -> ROLE
            // =====================================================

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================================
            // ROLE PERMISSION PRIMARY KEY
            // =====================================================

            modelBuilder.Entity<RolePermission>()
                .HasKey(rp => new
                {
                    rp.RoleId,
                    rp.PermissionId
                });


            // =====================================================
            // ROLE -> ROLE PERMISSIONS
            // =====================================================

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Role)
                .WithMany(r => r.RolePermissions)
                .HasForeignKey(rp => rp.RoleId)
                .OnDelete(DeleteBehavior.Cascade);


            // =====================================================
            // PERMISSION -> ROLE PERMISSIONS
            // =====================================================

            modelBuilder.Entity<RolePermission>()
                .HasOne(rp => rp.Permission)
                .WithMany(p => p.RolePermissions)
                .HasForeignKey(rp => rp.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);


            // =====================================================
            // CATEGORY -> ASSETS
            // =====================================================

            modelBuilder.Entity<Assest>()
                .HasOne(a => a.Category)
                .WithMany(static c => c.Assets)
                .HasForeignKey(a => a.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================================
            // USER -> ASSIGNED ASSETS
            // =====================================================

            modelBuilder.Entity<Assest>()
                .HasOne(a => a.AssignedToUser)
                .WithMany(u => u.Assets)
                .HasForeignKey(a => a.AssignedToUserId)
                .OnDelete(DeleteBehavior.SetNull);


            // =====================================================
            // USER -> NOTIFICATIONS
            // =====================================================

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // =====================================================
            // USER -> AUDIT LOGS
            // =====================================================

            modelBuilder.Entity<AuditLogEntry>()
                .HasOne(a => a.CreatedByUser)
                .WithMany(u => u.AuditLogs)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // =====================================================
            // UNIQUE USERNAME
            // =====================================================

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();


            // =====================================================
            // UNIQUE EMAIL
            // =====================================================

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();


            // =====================================================
            // ASSET PURCHASE COST
            // =====================================================

            modelBuilder.Entity<Assest>()
                .Property(a => a.PurchaseCost)
                .HasPrecision(18, 2);


            // =====================================================
            // UNIQUE ASSET CODE
            // =====================================================

            modelBuilder.Entity<Assest>()
                .HasIndex(a => a.AssestCode)
                .IsUnique();


            // =====================================================
            // UNIQUE CATEGORY NAME
            // =====================================================

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();


            // =====================================================
            // UNIQUE ROLE NAME
            // =====================================================

            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Name)
                .IsUnique();
        }
    }
}
