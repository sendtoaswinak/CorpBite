using CorpBite.Data;
using CorpBite.Models;
using CorpBite.ViewModels.AuthViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;

namespace CorpBite.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email address is already registered.");
                    return View(model);
                }

                // Password hashing
                using var hmac = new HMACSHA256();
                var passwordSalt = hmac.Key;
                var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));

                var newUser = new User
                {
                    Email = model.Email,
                    PasswordHash = Convert.ToBase64String(passwordHash) + "|" + Convert.ToBase64String(passwordSalt),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Role = "Employee" // Set default role
                    
                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Authenticate the user after registration
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                    new Claim(ClaimTypes.Name, newUser.Email),
                    new Claim(ClaimTypes.Role, newUser.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, "applicationCookie");
                var principal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync("applicationCookie", principal, new AuthenticationProperties { IsPersistent = false });

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

                if (user != null)
                {
                    var passwordParts = user.PasswordHash.Split('|');
                    var storedHash = Convert.FromBase64String(passwordParts[0]);
                    var storedSalt = Convert.FromBase64String(passwordParts[1]);

                    using var hmac = new HMACSHA256(storedSalt);
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));

                    if (computedHash.SequenceEqual(storedHash))
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.Email),
                            new Claim(ClaimTypes.Role, user.Role)
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, "applicationCookie");
                        var principal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignInAsync("applicationCookie", principal, new AuthenticationProperties
                        {
                            IsPersistent = model.RememberMe
                        });

                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("applicationCookie");
            return RedirectToAction("Index", "Home");
        }
    }
}