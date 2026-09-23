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
            // If already logged in, go directly to Dashboard
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

            // Validate login form
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Remove accidental spaces
            string loginValue = model.Username.Trim();

            // Find user using username OR email
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.Username == loginValue ||
                    u.Email == loginValue);

            // User not found
            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid username or password.");

                return View(model);
            }

            // Check whether account is active
            if (!user.IsActive)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account has been deactivated.");

                return View(model);
            }

            // Check whether the user has a valid role
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


            // Create identity
            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);


            // Create principal
            var principal = new ClaimsPrincipal(identity);


            // Authentication properties
            var properties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                AllowRefresh = true
            };


            // Sign in user
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                properties);


            // =====================================================
            // REDIRECT AFTER LOGIN
            // =====================================================

            // Return to originally requested local page
            if (!string.IsNullOrWhiteSpace(returnUrl)
                && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // Otherwise go to Dashboard
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
            // Already logged-in users don't need registration
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
            // Validate registration form
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

            // New public registrations are assigned
            // Inventory Executive automatically.
            //
            // Asset Manager accounts should be created
            // by an authorized administrator.

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

                // NEVER store plain-text passwords.
                // BCrypt creates a secure password hash.
                PasswordHash =
                    BCrypt.Net.BCrypt.HashPassword(
                        model.Password),

                RoleId = role.Id,

                IsActive = true,

                CreatedDate = DateTime.UtcNow
            };


            // Add user to database
            _context.Users.Add(user);


            // Save changes
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
            // Remove authentication cookie
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            // Return to login page
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
