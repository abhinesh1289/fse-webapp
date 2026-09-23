using System.Security.Claims;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystem.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // MY PROFILE - GET
        // =========================================================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int? userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user == null)
            {
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction("Login", "Account");
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                RoleName = user.Role?.Name ?? "User",
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate
            };

            return View(model);
        }

        // =========================================================
        // MY PROFILE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            int? userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user == null)
            {
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction("Login", "Account");
            }

            // Username should always come from database.
            model.Username = user.Username;

            // These fields are display-only.
            model.RoleName = user.Role?.Name ?? "User";
            model.IsActive = user.IsActive;
            model.CreatedDate = user.CreatedDate;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.Email.Trim();
            string fullName = model.FullName.Trim();

            // Check duplicate email.
            bool emailExists = await _context.Users
                .AnyAsync(u =>
                    u.Id != user.Id &&
                    u.Email == email);

            if (emailExists)
            {
                ModelState.AddModelError(
                    "Email",
                    "This email address is already in use.");

                return View(model);
            }

            // Update only fields that the user is allowed to change.
            user.FullName = fullName;
            user.Email = email;

            await _context.SaveChangesAsync();

            // =====================================================
            // Refresh login claims
            // =====================================================

            var claims = new List<Claim>
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Username),

                new Claim(
                    ClaimTypes.GivenName,
                    user.FullName),

                new Claim(
                    ClaimTypes.Email,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role?.Name ?? "User")
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            var authenticationProperties =
                await HttpContext.AuthenticateAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                authenticationProperties.Properties);

            TempData["SuccessMessage"] =
                "Profile updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // CHANGE PASSWORD - GET
        // =========================================================

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // =========================================================
        // CHANGE PASSWORD - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int? userId = GetUserId();

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId.Value);

            if (user == null)
            {
                await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction("Login", "Account");
            }

            bool passwordValid;

            try
            {
                passwordValid = BCrypt.Net.BCrypt.Verify(
                    model.CurrentPassword,
                    user.PasswordHash);
            }
            catch
            {
                passwordValid = false;
            }

            if (!passwordValid)
            {
                ModelState.AddModelError(
                    "CurrentPassword",
                    "Current password is incorrect.");

                return View(model);
            }

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(
                    model.NewPassword);

            await _context.SaveChangesAsync();

            // Force user to login again after password change.
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            TempData["SuccessMessage"] =
                "Password changed successfully. Please login again.";

            return RedirectToAction("Login", "Account");
        }

        // =========================================================
        // GET CURRENT USER ID
        // =========================================================

        private int? GetUserId()
        {
            var claim = User.FindFirst(
                ClaimTypes.NameIdentifier);

            if (claim == null)
            {
                return null;
            }

            if (int.TryParse(claim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}