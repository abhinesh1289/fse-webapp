using Inventory_Managgement1.Models;

namespace Inventory_Managgement1.Services
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default);
    }
}
