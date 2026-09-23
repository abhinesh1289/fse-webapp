using InventoryManagementSystem.Services;
using InventoryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            CancellationToken cancellationToken)
        {
            var model = await _dashboardService
                .GetDashboardAsync(cancellationToken);

            model.UserName =
                User.FindFirstValue(ClaimTypes.GivenName)
                ?? User.Identity?.Name
                ?? "User";

            model.UserRole =
                User.FindFirstValue(ClaimTypes.Role)
                ?? "User";

            return View(model);
        }
    }
}
