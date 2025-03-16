// RestaurantController.cs
using CorpBite.Data;
using CorpBite.Models;
using CorpBite.ViewModels.RestaurantViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CorpBite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RestaurantController : Controller
    {
        private readonly AppDbContext _context;

        public RestaurantController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var restaurants = await _context.Restaurants
                .Include(r => r.Location)
                .ToListAsync();
            return View(restaurants);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var locations = await _context.Locations.ToListAsync();
            ViewBag.Locations = locations.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = $"{l.BuildingName} - Floor {l.FloorNumber}" 
            }).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RestaurantViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get the ID of the currently logged-in user
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int adminUserId))
                {
                    var restaurant = new Restaurant
                    {
                        Name = model.Name,
                        Description = model.Description,
                        LocationId = model.LocationId,
                        AdminUserId = adminUserId // Set the AdminUserId
                    };
                    _context.Restaurants.Add(restaurant);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                // If we can't get the user ID, add an error to the model state
                ModelState.AddModelError(string.Empty, "Could not retrieve the current user's ID.");
            }
            ViewBag.Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "BuildingName", model.LocationId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null)
            {
                return NotFound();
            }
            var model = new RestaurantViewModel
            {
                Id = restaurant.Id,
                Name = restaurant.Name,
                Description = restaurant.Description,
                LocationId = restaurant.LocationId
            };
            ViewBag.Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "BuildingName", model.LocationId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RestaurantViewModel model)
        {
            if (ModelState.IsValid)
            {
                var restaurant = await _context.Restaurants.FindAsync(id);
                if (restaurant == null)
                {
                    return NotFound();
                }
                restaurant.Name = model.Name;
                restaurant.Description = model.Description;
                restaurant.LocationId = model.LocationId;
                restaurant.UpdatedOn = System.DateTime.UtcNow;
                _context.Restaurants.Update(restaurant);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Locations = new SelectList(await _context.Locations.ToListAsync(), "Id", "BuildingName", model.LocationId);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var restaurant = await _context.Restaurants
                .Include(r => r.Location)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return View(restaurant);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant != null)
            {
                _context.Restaurants.Remove(restaurant);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Menu(int restaurantId)
        {
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null)
            {
                return NotFound();
            }

            var menuItems = await _context.MenuItems
                .Where(m => m.RestaurantId == restaurantId)
                .Select(m => new MenuItemViewModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Description = m.Description,
                    Price = m.Price,
                    Category = m.Category,
                    IsActive = m.IsActive,
                    RestaurantId = m.RestaurantId,
                    RestaurantName = restaurant.Name
                })
                .ToListAsync();

            ViewBag.RestaurantName = restaurant.Name;
            ViewBag.RestaurantId = restaurantId;

            return View(menuItems);
        }

        [HttpGet]
        public IActionResult CreateMenuItem(int restaurantId)
        {
            ViewBag.RestaurantId = restaurantId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMenuItem(int restaurantId, MenuItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var menuItem = new MenuItem
                {
                    RestaurantId = restaurantId,
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    Category = model.Category,
                    IsActive = model.IsActive
                };

                _context.MenuItems.Add(menuItem);
                await _context.SaveChangesAsync();

                return RedirectToAction("Menu", new { restaurantId = restaurantId });
            }
            ViewBag.RestaurantId = restaurantId;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            var model = new MenuItemViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                Category = menuItem.Category,
                IsActive = menuItem.IsActive,
                RestaurantId = menuItem.RestaurantId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMenuItem(int id, MenuItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var menuItem = await _context.MenuItems.FindAsync(id);
                if (menuItem == null)
                {
                    return NotFound();
                }

                menuItem.Name = model.Name;
                menuItem.Description = model.Description;
                menuItem.Price = model.Price;
                menuItem.Category = model.Category;
                menuItem.IsActive = model.IsActive;
                menuItem.UpdatedOn = System.DateTime.UtcNow;

                _context.MenuItems.Update(menuItem);
                await _context.SaveChangesAsync();

                return RedirectToAction("Menu", new { restaurantId = model.RestaurantId });
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnableMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            menuItem.IsActive = true;
            menuItem.UpdatedOn = System.DateTime.UtcNow;
            _context.MenuItems.Update(menuItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Menu", new { restaurantId = menuItem.RestaurantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DisableMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            menuItem.IsActive = false;
            menuItem.UpdatedOn = System.DateTime.UtcNow;
            _context.MenuItems.Update(menuItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Menu", new { restaurantId = menuItem.RestaurantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            var restaurantId = menuItem.RestaurantId;
            _context.MenuItems.Remove(menuItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Menu", new { restaurantId = restaurantId });
        }


        [HttpGet]
        public async Task<IActionResult> DeleteMenuItemWarning(int id)
        {
            var menuItem = await _context.MenuItems.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            var restaurant = await _context.Restaurants.FindAsync(menuItem.RestaurantId);
            if (restaurant == null)
            {
                return NotFound();
            }

            var model = new MenuItemViewModel
            {
                Id = menuItem.Id,
                Name = menuItem.Name,
                Description = menuItem.Description,
                Price = menuItem.Price,
                Category = menuItem.Category,
                IsActive = menuItem.IsActive,
                RestaurantId = menuItem.RestaurantId,
                RestaurantName = restaurant.Name
            };

            return View(model);
        }

    }
}