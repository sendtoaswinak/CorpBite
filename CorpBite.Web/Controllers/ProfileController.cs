using CorpBite.Web.ViewModels.Profile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CorpBite.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DateTime _currentDateTime = DateTime.Parse("2025-03-14 07:36:01");
        private readonly string _currentUser = "sendtoaswinak";

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _context.Users
                .Include(u => u.Orders.OrderByDescending(o => o.OrderDate).Take(5))
                    .ThenInclude(o => o.FoodCourt)
                .Include(u => u.FavoriteItems)
                    .ThenInclude(f => f.MenuItem)
                        .ThenInclude(mi => mi.FoodCourt)
                .FirstOrDefaultAsync(u => u.Id == _currentUser);

            if (user == null)
                return NotFound();

            var viewModel = new UserProfileViewModel
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                EmployeeId = user.EmployeeId,
                Department = user.Department,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                RecentOrders = user.Orders.Select(o => new OrderViewModel
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    FoodCourtName = o.FoodCourt.Name,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus
                }).ToList(),
                FavoriteItems = user.FavoriteItems.Select(f => new FavoriteItemViewModel
                {
                    MenuItemId = f.MenuItemId,
                    Name = f.MenuItem.Name,
                    Description = f.MenuItem.Description,
                    Price = f.MenuItem.Price,
                    ImageUrl = f.MenuItem.ImageUrl,
                    IsVeg = f.MenuItem.IsVeg,
                    IsAvailable = f.MenuItem.IsAvailable,
                    FoodCourtName = f.MenuItem.FoodCourt.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FindAsync(_currentUser);
            if (user == null)
                return NotFound();

            if (!VerifyPassword(model.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError("CurrentPassword", "Current password is incorrect");
                return View(model);
            }

            user.PasswordHash = HashPassword(model.NewPassword);
            user.UpdatedAt = _currentDateTime;
            user.UpdatedBy = _currentUser;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password changed successfully";
            return RedirectToAction(nameof(Index));
        }

        private string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}