using Inventory_Managgement1.Models;
using Inventory_Managgement1.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Inventory_Managgement1.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = await _dashboardService.GetDashboardAsync(cancellationToken);
            model.UserName = GetDisplayName();
            model.UserRole = GetDisplayRole();

            ViewData["UserName"] = model.UserName;
            ViewData["UserRole"] = model.UserRole;

            return View(model);
        }

        private string? GetDisplayName()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return User.Identity?.Name;
            }

            return User.FindFirstValue(ClaimTypes.GivenName)
                   ?? User.FindFirstValue("name")
                   ?? User.Identity?.Name;
        }

        private string? GetDisplayRole()
        {
            return User.FindFirstValue(ClaimTypes.Role)
                   ?? User.FindFirstValue("role");
        }
    }
}
