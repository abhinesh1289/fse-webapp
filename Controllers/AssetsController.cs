using InventoryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class AssetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AssetsController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // ASSET LIST
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var assets = await _context.Assets
                .Include(a => a.Category)
                .Include(a => a.AssignedToUser)
                .OrderBy(a => a.AssestCode)
                .ToListAsync();

            return View(assets);
        }
    }
}
