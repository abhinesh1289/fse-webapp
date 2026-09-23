using InventoryManagementSystem.ViewModels;

namespace InventoryManagementSystem.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardAsync(
            CancellationToken cancellationToken);
    }
}
