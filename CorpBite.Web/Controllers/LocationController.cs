using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CorpBite.Web.Controllers
{
    [Authorize]
    public class LocationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DateTime _currentDateTime = DateTime.Parse("2025-03-14 07:36:01");
        private readonly string _currentUser = "sendtoaswinak";

        public LocationController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var locations = await _context.Locations
                .Where(l => l.IsActive)
                .Include(l => l.FoodCourts.Where(fc => fc.IsActive))
                .OrderBy(l => l.BuildingName)
                .ThenBy(l => l.FloorNumber)
                .ToListAsync();

            var viewModel = locations.Select(l => new LocationViewModel
            {
                Id = l.Id,
                BuildingName = l.BuildingName,
                FloorNumber = l.FloorNumber,
                Description = l.Description,
                FoodCourts = l.FoodCourts.Select(fc => new FoodCourtBasicViewModel
                {
                    Id = fc.Id,
                    Name = fc.Name,
                    ImageUrl = fc.ImageUrl,
                    IsVeg = fc.IsVeg,
                    IsNonVeg = fc.IsNonVeg,
                    Rating = fc.Rating,
                    TotalReviews = fc.TotalReviews
                }).ToList()
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var location = await _context.Locations
                .Include(l => l.FoodCourts.Where(fc => fc.IsActive))
                    .ThenInclude(fc => fc.MenuItems.Where(mi => mi.IsActive && mi.IsAvailable))
                .FirstOrDefaultAsync(l => l.Id == id && l.IsActive);

            if (location == null)
                return NotFound();

            var viewModel = new LocationDetailsViewModel
            {
                Id = location.Id,
                BuildingName = location.BuildingName,
                FloorNumber = location.FloorNumber,
                Description = location.Description,
                FoodCourts = location.FoodCourts.Select(fc => new FoodCourtDetailViewModel
                {
                    Id = fc.Id,
                    Name = fc.Name,
                    Description = fc.Description,
                    ImageUrl = fc.ImageUrl,
                    IsVeg = fc.IsVeg,
                    IsNonVeg = fc.IsNonVeg,
                    ContactNumber = fc.ContactNumber,
                    OpeningTime = fc.OpeningTime,
                    ClosingTime = fc.ClosingTime,
                    Rating = fc.Rating,
                    TotalReviews = fc.TotalReviews,
                    PopularItems = fc.MenuItems
                        .OrderByDescending(mi => mi.Rating)
                        .Take(4)
                        .Select(mi => new MenuItemBasicViewModel
                        {
                            Id = mi.Id,
                            Name = mi.Name,
                            Price = mi.Price,
                            ImageUrl = mi.ImageUrl,
                            IsVeg = mi.IsVeg,
                            Rating = mi.Rating
                        }).ToList()
                }).ToList()
            };

            return View(viewModel);
        }
    }
}