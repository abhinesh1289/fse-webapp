using InventoryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
    [Authorize(Roles = "Asset Manager")]
    public class OrganisationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrganisationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var organisation = await _context.Organisations
                .OrderBy(static o => o.Id)
                .FirstOrDefaultAsync();

            return View(organisation);
        }
    }
}