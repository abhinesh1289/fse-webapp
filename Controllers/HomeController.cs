using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            ViewData["Title"] = "Error";
            ViewBag.StatusCode = statusCode;

            return View();
        }
    }
}
