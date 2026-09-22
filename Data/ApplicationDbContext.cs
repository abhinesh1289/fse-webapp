using Inventory_Managgement1.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Managgement1.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<Category> Categories => Set<Category>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Categories");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            });

            modelBuilder.Entity<Asset>(entity =>
            {
                entity.ToTable("Assets");
                entity.HasKey(x => x.Id);
                entity.Property(x => x.AssetTag).HasMaxLength(50).IsRequired();
                entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
                entity.Property(x => x.Status).HasMaxLength(50).IsRequired();
                entity.Property(x => x.AssignedTo).HasMaxLength(150);
                entity.HasIndex(x => x.AssetTag).IsUnique();
                entity.HasIndex(x => x.Status);
                entity.HasOne(x => x.Category)
                    .WithMany(x => x.Assets)
                    .HasForeignKey(x => x.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
