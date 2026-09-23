using InventoryManagementSystem.Data;
using InventoryManagementSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(
            ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardViewModel> GetDashboardAsync(
            CancellationToken cancellationToken)
        {
            var model = new DashboardViewModel();

            // =====================================================
            // TOTAL ASSETS
            // =====================================================

            model.TotalAssets =
                await _context.Assets
                    .CountAsync(cancellationToken);


            // =====================================================
            // ASSET STATUS COUNTS
            // =====================================================

            model.AvailableAssets =
                await _context.Assets
                    .CountAsync(
                        a => a.Status == "Available",
                        cancellationToken);

            model.AssignedAssets =
                await _context.Assets
                    .CountAsync(
                        a => a.Status == "Assigned",
                        cancellationToken);

            model.MaintenanceAssets =
                await _context.Assets
                    .CountAsync(
                        a => a.Status == "Under Maintenance",
                        cancellationToken);

            model.DamagedAssets =
                await _context.Assets
                    .CountAsync(
                        a => a.Status == "Damaged",
                        cancellationToken);

            model.RetiredAssets =
                await _context.Assets
                    .CountAsync(
                        a => a.Status == "Retired",
                        cancellationToken);


            // =====================================================
            // CATEGORY SUMMARY
            // =====================================================

            model.CategorySummary =
                await _context.Assets
                    .Include(a => a.Category)
                    .GroupBy(a => a.Category.Name)
                    .Select(g => new CategorySummary
                    {
                        CategoryName = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync(cancellationToken);


            // =====================================================
            // STATUS SUMMARY
            // =====================================================

            var statusData =
                await _context.Assets
                    .GroupBy(a => a.Status)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .ToListAsync(cancellationToken);


            model.StatusSummary =
                statusData
                    .Select(x => new StatusSummary
                    {
                        Status = x.Status,

                        Count = x.Count,

                        Percentage =
                            model.TotalAssets == 0
                                ? 0
                                : Math.Round(
                                    (double)x.Count
                                    / model.TotalAssets
                                    * 100,
                                    2)
                    })
                    .ToList();


            // =====================================================
            // RECENT ACTIVITIES
            // =====================================================

            model.RecentActivities =
                new List<RecentActivity>();


            return model;
        }
    }
}
