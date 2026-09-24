using System.Security.Claims;
using InventoryManagementSystem.Data;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace InventoryManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // LOGIN - GET
        // =========================================================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard");
            }
            ViewBag.ReturnUrl = returnUrl;

            return View();
        }
        // =========================================================
        // LOGIN - POST
        // =========================================================
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            string loginValue = model.Username.Trim();
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.Username == loginValue ||
                    u.Email == loginValue);
            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid username or password.");

                return View(model);
            }
            if (!user.IsActive)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account has been deactivated.");
                return View(model);
            }
            if (user.Role == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account does not have a valid role.");
                return View(model);
            }

            // =====================================================
            // VERIFY PASSWORD
            // =====================================================
            bool passwordValid;
            try
            {
                passwordValid =
                    BCrypt.Net.BCrypt.Verify(
                        model.Password,
                        user.PasswordHash);
            }
            catch
            {
                passwordValid = false;
            }

            if (!passwordValid)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid username or password.");

                return View(model);
            }
            // =====================================================
            // CREATE AUTHENTICATION CLAIMS
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
                    user.Role.Name)
            };
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var properties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                AllowRefresh = true
            };
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                properties);
            // =====================================================
            // REDIRECT AFTER LOGIN
            // =====================================================
            if (!string.IsNullOrWhiteSpace(returnUrl)
                && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(
                "Index",
                "Dashboard");
        }
        // =========================================================
        // REGISTER - GET
        // =========================================================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction(
                    "Index",
                    "Dashboard");
            }
            return View();
        }
        // =========================================================
        // REGISTER - POST
        // =========================================================
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // =====================================================
            // CLEAN USER INPUT
            // =====================================================
            string fullName = model.FullName.Trim();
            string username = model.Username.Trim();
            string email = model.Email.Trim();
            // =====================================================
            // CHECK USERNAME
            // =====================================================
            bool usernameExists =
                await _context.Users
                    .AnyAsync(u =>
                        u.Username == username);
            if (usernameExists)
            {
                ModelState.AddModelError(
                    "Username",
                    "Username already exists.");

                return View(model);
            }
            // =====================================================
            // CHECK EMAIL
            // =====================================================
            bool emailExists =
                await _context.Users
                    .AnyAsync(u =>
                        u.Email == email);
            if (emailExists)
            {
                ModelState.AddModelError(
                    "Email",
                    "Email already exists.");

                return View(model);
            }
            // =====================================================
            // GET DEFAULT ROLE
            // =====================================================
            var role = await _context.Roles
                .FirstOrDefaultAsync(r =>
                    r.Name == "Inventory Executive"
                    && r.IsActive);
            if (role == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Default user role is not configured. " +
                    "Please contact the administrator.");
                return View(model);
            }
            // =====================================================
            // CREATE USER
            // =====================================================
            var user = new User
            {
                FullName = fullName,
                Username = username,
                Email = email,
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        model.Password),
                RoleId = role.Id,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // =====================================================
            // REGISTRATION SUCCESS
            // =====================================================
            TempData["SuccessMessage"] =
                "Registration successful. " +
                "You can now login.";
            return RedirectToAction(
                "Login",
                "Account");
        }
        // =========================================================
        // LOGOUT
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(
                "Login",
                "Account");
        }
        // =========================================================
        // ACCESS DENIED
        // =========================================================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
