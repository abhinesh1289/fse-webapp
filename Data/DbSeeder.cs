using InventoryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagementSystem.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            ApplicationDbContext context)
        {
            await context.Database.MigrateAsync();
            var assetManagerRole =
                await context.Roles
                    .FirstOrDefaultAsync(
                        r => r.Name == "Asset Manager");
            if (assetManagerRole == null)
            {
                assetManagerRole = new Role
                {
                    Name = "Asset Manager",
                    Description = "Administrative user",
                    IsActive = true
                };
                context.Roles.Add(assetManagerRole);
                await context.SaveChangesAsync();
            }
            var inventoryExecutiveRole =
                await context.Roles
                    .FirstOrDefaultAsync(
                        r => r.Name == "Inventory Executive");
            if (inventoryExecutiveRole == null)
            {
                inventoryExecutiveRole = new Role
                {
                    Name = "Inventory Executive",
                    Description = "Day-to-day inventory user",
                    IsActive = true
                };
                context.Roles.Add(inventoryExecutiveRole);
                await context.SaveChangesAsync();
            }
            var permissionData = new[]
            {
                new
                {
                    Name = "View Assets",
                    Description = "View inventory assets"
                },
                new
                {
                    Name = "Create Assets",
                    Description = "Create inventory assets"
                },
                new
                {
                    Name = "Edit Assets",
                    Description = "Edit inventory assets"
                },
                new
                {
                    Name = "Delete Assets",
                    Description = "Delete inventory assets"
                },
                new
                {
                    Name = "Manage Users",
                    Description = "Manage system users"
                },
                new
                {
                    Name = "Manage Roles",
                    Description = "Manage roles and permissions"
                },
                new
                {
                    Name = "Manage Categories",
                    Description = "Manage asset categories"
                },
                new
                {
                    Name = "View Reports",
                    Description = "View inventory reports"
                }
            };
            foreach (var item in permissionData)
            {
                bool exists =
                    await context.Permissions
                        .AnyAsync(p => p.Name == item.Name);
                if (!exists)
                {
                    context.Permissions.Add(
                        new Permission
                        {
                            Name = item.Name,
                            Description = item.Description
                        });
                }
            }
            await context.SaveChangesAsync();
            var permissions =
                await context.Permissions.ToListAsync();
            foreach (var permission in permissions)
            {
                bool exists =
                    await context.RolePermissions
                        .AnyAsync(rp =>
                            rp.RoleId == assetManagerRole.Id &&
                            rp.PermissionId == permission.Id);
                if (!exists)
                {
                    context.RolePermissions.Add(
                        new RolePermission
                        {
                            RoleId = assetManagerRole.Id,
                            PermissionId = permission.Id
                        });
                }
            }
            var executivePermissionNames =
                new[]
                {
                    "View Assets",
                    "Create Assets",
                    "Edit Assets",
                    "View Reports"
                };
            foreach (var permissionName
                     in executivePermissionNames)
            {
                var permission =
                    permissions.FirstOrDefault(
                        p => p.Name == permissionName);

                if (permission == null)
                {
                    continue;
                }
                bool exists =
                    await context.RolePermissions
                        .AnyAsync(rp =>
                            rp.RoleId ==
                                inventoryExecutiveRole.Id &&
                            rp.PermissionId ==
                                permission.Id);
                if (!exists)
                {
                    context.RolePermissions.Add(
                        new RolePermission
                        {
                            RoleId =
                                inventoryExecutiveRole.Id,

                            PermissionId =
                                permission.Id
                        });
                }
            }
            await context.SaveChangesAsync();
            var admin =
                await context.Users
                    .FirstOrDefaultAsync(
                        u => u.Username == "admin");
            if (admin == null)
            {
                admin = new User
                {
                    Username = "admin",

                    FullName =
                        "System Administrator",

                    Email =
                        "admin@inventory.com",

                    PasswordHash =
                        BCrypt.Net.BCrypt.HashPassword(
                            "Admin@123"),

                    RoleId =
                        assetManagerRole.Id,

                    IsActive = true,

                    CreatedDate =
                        DateTime.UtcNow
                };

                context.Users.Add(admin);

                await context.SaveChangesAsync();
            }
            var categories = new[]
            {
                new
                {
                    Name = "Laptop",
                    Description = "Laptop computers"
                },
                new
                {
                    Name = "Desktop",
                    Description = "Desktop computers"
                },
                new
                {
                    Name = "Printer",
                    Description = "Printers"
                }
            };
            foreach (var item in categories)
            {
                bool exists =
                    await context.Categories
                        .AnyAsync(
                            c => c.Name == item.Name);
                if (!exists)
                {
                    context.Categories.Add(
                        new Category
                        {
                            Name = item.Name,

                            Description =
                                item.Description,

                            IsActive = true
                        });
                }
            }
            await context.SaveChangesAsync();
            bool organisationExists =
                await context.Organisations.AnyAsync();
            if (!organisationExists)
            {
                context.Organisations.Add(
                    new Organisation
                    {
                        Name = "My Organisation",

                        Email =
                            "admin@inventory.com"
                    });
                await context.SaveChangesAsync();
            }
        }
    }
}
