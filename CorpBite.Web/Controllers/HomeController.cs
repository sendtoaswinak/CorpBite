using CorpBite.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CorpBite.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DateTime _currentDateTime = DateTime.Parse("2025-03-14 07:34:58");
        private readonly string _currentUser = "sendtoaswinak";

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var foodCourts = await _context.FoodCourts
                .Include(fc => fc.Location)
                .Where(fc => fc.IsActive)
                .OrderByDescending(fc => fc.Rating)
                .Take(6)
                .Select(fc => new FoodCourtViewModel
                {
                    Id = fc.Id,
                    Name = fc.Name,
                    Description = fc.Description,
                    ImageUrl = fc.ImageUrl,
                    IsVeg = fc.IsVeg,
                    IsNonVeg = fc.IsNonVeg,
                    Rating = fc.Rating,
                    TotalReviews = fc.TotalReviews,
                    Location = new LocationViewModel
                    {
                        BuildingName = fc.Location.BuildingName,
                        FloorNumber = fc.Location.FloorNumber,
                        Description = fc.Location.Description
                    }
                })
                .ToListAsync();

            var popularItems = await _context.MenuItems
                .Include(mi => mi.FoodCourt)
                .Where(mi => mi.IsActive && mi.IsAvailable)
                .OrderByDescending(mi => mi.Rating)
                .Take(8)
                .Select(mi => new MenuItemViewModel
                {
                    Id = mi.Id,
                    Name = mi.Name,
                    Description = mi.Description,
                    Price = mi.Price,
                    ImageUrl = mi.ImageUrl,
                    IsVeg = mi.IsVeg,
                    Rating = mi.Rating,
                    ReviewCount = mi.ReviewCount,
                    FoodCourtName = mi.FoodCourt.Name
                })
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                FeaturedFoodCourts = foodCourts,
                PopularItems = popularItems
            };

            if (User.Identity?.IsAuthenticated ?? false)
            {
                viewModel.RecentOrders = await _context.Orders
                    .Include(o => o.FoodCourt)
                    .Where(o => o.UserId == _currentUser)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(3)
                    .Select(o => new OrderViewModel
                    {
                        Id = o.Id,
                        OrderNumber = o.OrderNumber,
                        FoodCourtName = o.FoodCourt.Name,
                        OrderDate = o.OrderDate,
                        TotalAmount = o.TotalAmount,
                        OrderStatus = o.OrderStatus
                    })
                    .ToListAsync();
            }

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}