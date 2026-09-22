using Inventory_Managgement1.Data;
using Inventory_Managgement1.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Managgement1.Services
{
    public class DashboardService : IDashboardService
    {
        private static readonly string[] CategoryPalette =
        {
            "#2563eb", "#7c3aed", "#0f9d8e", "#f59e0b", "#06b6d4", "#64748b", "#db2777", "#84cc16"
        };

        private static readonly Dictionary<string, string> StatusColors = new(StringComparer.OrdinalIgnoreCase)
        {
            [AssetStatuses.Available] = "#16a34a",
            [AssetStatuses.Assigned] = "#2563eb",
            [AssetStatuses.Maintenance] = "#d97706",
            [AssetStatuses.Damaged] = "#dc2626",
            [AssetStatuses.Retired] = "#64748b"
        };

        private readonly ApplicationDbContext _context;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(ApplicationDbContext context, ILogger<DashboardService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
        {
            var assets = _context.Assets.AsNoTracking();

            var statusRows = await assets
                .GroupBy(a => a.Status)
                .Select(g => new StatusCountRow(g.Key, g.Count()))
                .ToListAsync(cancellationToken);

            var totalAssets = statusRows.Sum(x => x.Count);

            var categoryRows = await assets
                .GroupBy(a => a.Category.Name)
                .Select(g => new { CategoryName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync(cancellationToken);

            var recentAssets = await assets
                .OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt)
                .Take(5)
                .Select(a => new RecentAssetRow(
                    a.AssetTag,
                    a.Status,
                    a.AssignedTo,
                    a.CreatedAt,
                    a.UpdatedAt))
                .ToListAsync(cancellationToken);

            var model = new DashboardViewModel
            {
                TotalAssets = totalAssets,
                AvailableAssets = CountStatus(statusRows, AssetStatuses.Available),
                AssignedAssets = CountStatus(statusRows, AssetStatuses.Assigned),
                MaintenanceAssets = CountStatus(statusRows, AssetStatuses.Maintenance),
                DamagedAssets = CountStatus(statusRows, AssetStatuses.Damaged),
                RetiredAssets = CountStatus(statusRows, AssetStatuses.Retired),
                CategorySummary = categoryRows
                    .Select((row, index) => new CategorySummary
                    {
                        CategoryName = string.IsNullOrWhiteSpace(row.CategoryName) ? "Uncategorized" : row.CategoryName,
                        Count = row.Count,
                        Color = CategoryPalette[index % CategoryPalette.Length]
                    })
                    .ToList(),
                StatusSummary = statusRows
                    .OrderByDescending(x => x.Count)
                    .Select(row => new StatusSummary
                    {
                        Status = string.IsNullOrWhiteSpace(row.Status) ? "Unknown" : row.Status,
                        Count = row.Count,
                        Percentage = totalAssets == 0 ? 0 : Math.Round((double)row.Count / totalAssets * 100, 1),
                        Color = StatusColors.TryGetValue(row.Status ?? string.Empty, out var color) ? color : "#64748b"
                    })
                    .ToList(),
                RecentActivities = recentAssets
                    .Select(BuildActivity)
                    .ToList()
            };

            _logger.LogInformation("Loaded dashboard for {Total} assets.", totalAssets);
            return model;
        }

        private static int CountStatus(IReadOnlyCollection<StatusCountRow> rows, string status)
        {
            var match = rows.FirstOrDefault(x => string.Equals(x.Status, status, StringComparison.OrdinalIgnoreCase));
            return match?.Count ?? 0;
        }

        private static RecentActivity BuildActivity(RecentAssetRow asset)
        {
            var occurredAt = asset.UpdatedAt ?? asset.CreatedAt;
            var isNew = asset.UpdatedAt == null || asset.UpdatedAt.Value == asset.CreatedAt;

            string description;
            string icon;

            if (isNew)
            {
                description = $"New asset ({asset.AssetTag}) added to inventory";
                icon = "bi-plus-circle";
            }
            else if (string.Equals(asset.Status, AssetStatuses.Assigned, StringComparison.OrdinalIgnoreCase))
            {
                description = string.IsNullOrWhiteSpace(asset.AssignedTo)
                    ? $"Asset ({asset.AssetTag}) assigned"
                    : $"Asset ({asset.AssetTag}) assigned to {asset.AssignedTo}";
                icon = "bi-person-check";
            }
            else if (string.Equals(asset.Status, AssetStatuses.Maintenance, StringComparison.OrdinalIgnoreCase))
            {
                description = $"Asset ({asset.AssetTag}) status changed to Maintenance";
                icon = "bi-tools";
            }
            else if (string.Equals(asset.Status, AssetStatuses.Damaged, StringComparison.OrdinalIgnoreCase))
            {
                description = $"Asset ({asset.AssetTag}) marked as Damaged";
                icon = "bi-exclamation-triangle";
            }
            else if (string.Equals(asset.Status, AssetStatuses.Retired, StringComparison.OrdinalIgnoreCase))
            {
                description = $"Asset ({asset.AssetTag}) retired from inventory";
                icon = "bi-archive";
            }
            else
            {
                description = $"Asset ({asset.AssetTag}) updated";
                icon = "bi-pencil-square";
            }

            return new RecentActivity
            {
                Description = description,
                Time = ToRelativeTime(occurredAt),
                Icon = icon
            };
        }

        private static string ToRelativeTime(DateTime value)
        {
            var utc = value.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(value, DateTimeKind.Utc)
                : value.ToUniversalTime();
            var elapsed = DateTime.UtcNow - utc;

            if (elapsed.TotalMinutes < 1)
            {
                return "Just now";
            }

            if (elapsed.TotalMinutes < 60)
            {
                var minutes = Math.Max(1, (int)elapsed.TotalMinutes);
                return minutes == 1 ? "1 minute ago" : $"{minutes} minutes ago";
            }

            if (elapsed.TotalHours < 24)
            {
                var hours = Math.Max(1, (int)elapsed.TotalHours);
                return hours == 1 ? "1 hour ago" : $"{hours} hours ago";
            }

            var days = Math.Max(1, (int)elapsed.TotalDays);
            return days == 1 ? "1 day ago" : $"{days} days ago";
        }

        private sealed record StatusCountRow(string? Status, int Count);

        private sealed record RecentAssetRow(
            string AssetTag,
            string Status,
            string? AssignedTo,
            DateTime CreatedAt,
            DateTime? UpdatedAt);
    }
}
