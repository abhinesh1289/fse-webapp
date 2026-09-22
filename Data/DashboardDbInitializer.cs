using Inventory_Managgement1.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Managgement1.Data
{
    public static class DashboardDbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context)
        {
            await context.Database.EnsureCreatedAsync();

            if (await context.Assets.AnyAsync())
            {
                return;
            }

            var now = DateTime.UtcNow;

            var categories = new List<Category>
            {
                new() { Name = "Laptop" },
                new() { Name = "Desktop" },
                new() { Name = "Printer" },
                new() { Name = "Monitor" },
                new() { Name = "Projector" },
                new() { Name = "Other" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            var assets = new List<Asset>();
            var tagNumber = 1;

            void AddAssets(Category category, string status, int count, string? assignedTo, TimeSpan age)
            {
                for (var i = 0; i < count; i++)
                {
                    var created = now.Subtract(age).AddMinutes(-i * 7);
                    assets.Add(new Asset
                    {
                        AssetTag = $"A{tagNumber:000}",
                        Name = $"{category.Name} {tagNumber:000}",
                        Status = status,
                        AssignedTo = assignedTo,
                        CategoryId = category.Id,
                        CreatedAt = created,
                        UpdatedAt = status == AssetStatuses.Available ? created : created.AddHours(2 + (i % 5))
                    });
                    tagNumber++;
                }
            }

            var laptop = categories.First(c => c.Name == "Laptop");
            var desktop = categories.First(c => c.Name == "Desktop");
            var printer = categories.First(c => c.Name == "Printer");
            var monitor = categories.First(c => c.Name == "Monitor");
            var projector = categories.First(c => c.Name == "Projector");
            var other = categories.First(c => c.Name == "Other");

            AddAssets(laptop, AssetStatuses.Available, 18, null, TimeSpan.FromDays(12));
            AddAssets(laptop, AssetStatuses.Assigned, 12, "Ravi", TimeSpan.FromDays(8));
            AddAssets(laptop, AssetStatuses.Maintenance, 4, null, TimeSpan.FromHours(6));
            AddAssets(desktop, AssetStatuses.Available, 10, null, TimeSpan.FromDays(20));
            AddAssets(desktop, AssetStatuses.Assigned, 9, "Anita", TimeSpan.FromDays(3));
            AddAssets(desktop, AssetStatuses.Damaged, 2, null, TimeSpan.FromDays(2));
            AddAssets(printer, AssetStatuses.Available, 8, null, TimeSpan.FromDays(15));
            AddAssets(printer, AssetStatuses.Assigned, 5, "Karthik", TimeSpan.FromHours(26));
            AddAssets(printer, AssetStatuses.Retired, 1, null, TimeSpan.FromDays(40));
            AddAssets(monitor, AssetStatuses.Available, 9, null, TimeSpan.FromDays(9));
            AddAssets(monitor, AssetStatuses.Assigned, 6, "Meera", TimeSpan.FromHours(3));
            AddAssets(projector, AssetStatuses.Available, 4, null, TimeSpan.FromDays(5));
            AddAssets(projector, AssetStatuses.Maintenance, 3, null, TimeSpan.FromHours(1));
            AddAssets(other, AssetStatuses.Available, 6, null, TimeSpan.FromMinutes(18));
            AddAssets(other, AssetStatuses.Assigned, 4, "Suresh", TimeSpan.FromDays(1));

            context.Assets.AddRange(assets);
            await context.SaveChangesAsync();
        }
    }
}
