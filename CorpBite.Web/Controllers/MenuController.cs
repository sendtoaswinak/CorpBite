using CorpBite.Data;
using CorpBite.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CorpBite.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly AppDbContext _context;
        private readonly DateTime _currentDateTime = DateTime.Parse("2025-03-14 07:36:01");
        private readonly string _currentUser = "sendtoaswinak";

        public MenuController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int foodCourtId)
        {
            var foodCourt = await _context.FoodCourts
                .Include(fc => fc.Location)
                .Include(fc => fc.MenuItems.Where(mi => mi.IsActive))
                .FirstOrDefaultAsync(fc => fc.Id == foodCourtId && fc.IsActive);

            if (foodCourt == null)
                return NotFound();

            var menuCategories = foodCourt.MenuItems
                .GroupBy(mi => mi.Category)
                .Select(g => new MenuCategoryViewModel
                {
                    Name = g.Key,
                    Items = g.Select(mi => new MenuItemViewModel
                    {
                        Id = mi.Id,
                        Name = mi.Name,
                        Description = mi.Description,
                        Price = mi.Price,
                        ImageUrl = mi.ImageUrl,
                        IsVeg = mi.IsVeg,
                        IsAvailable = mi.IsAvailable,
                        Rating = mi.Rating,
                        ReviewCount = mi.ReviewCount
                    }).ToList()
                })
                .ToList();

            var cartItems = await _context.CartItems
                .Where(ci => ci.Cart.UserId == _currentUser && ci.Cart.FoodCourtId == foodCourtId)
                .Select(ci => new { ci.MenuItemId, ci.Quantity })
                .ToListAsync();

            var viewModel = new MenuViewModel
            {
                FoodCourtId = foodCourt.Id,
                FoodCourtName = foodCourt.Name,
                Location = new LocationViewModel
                {
                    BuildingName = foodCourt.Location.BuildingName,
                    FloorNumber = foodCourt.Location.FloorNumber
                },
                IsVeg = foodCourt.IsVeg,
                IsNonVeg = foodCourt.IsNonVeg,
                OpeningTime = foodCourt.OpeningTime,
                ClosingTime = foodCourt.ClosingTime,
                Rating = foodCourt.Rating,
                TotalReviews = foodCourt.TotalReviews,
                Categories = menuCategories,
                CartItems = cartItems.ToDictionary(ci => ci.MenuItemId, ci => ci.Quantity)
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetItemReviews(int id)
        {
            var reviews = await _context.Feedbacks
                .Include(f => f.User)
                .Where(f => f.MenuItemId == id)
                .OrderByDescending(f => f.CreatedAt)
                .Take(10)
                .Select(f => new ReviewViewModel
                {
                    UserName = f.User.Name,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();

            return Json(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleFavorite(int menuItemId)
        {
            var favorite = await _context.FavoriteItems
                .FirstOrDefaultAsync(f => f.UserId == _currentUser && f.MenuItemId == menuItemId);

            if (favorite != null)
            {
                _context.FavoriteItems.Remove(favorite);
            }
            else
            {
                _context.FavoriteItems.Add(new FavoriteItem
                {
                    UserId = _currentUser,
                    MenuItemId = menuItemId,
                    CreatedAt = _currentDateTime
                });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true, isFavorite = favorite == null });
        }
    }
}