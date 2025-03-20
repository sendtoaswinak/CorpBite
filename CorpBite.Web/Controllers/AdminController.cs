using CorpBite.Data;
using CorpBite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using CorpBite.ViewModels.AdminViewModels;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CorpBite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(); 
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard(string building, string floor, string menuItemName, string menuItemCategory, int? menuItemRestaurant)
        {
            var restaurantsQuery = _context.Restaurants.Include(r => r.Location);
            var menuItemsQuery = _context.MenuItems.Include(m => m.Restaurant);
            try
            {
                if (!string.IsNullOrEmpty(building))
                {
                    restaurantsQuery = (IIncludableQueryable<Restaurant, Location>)restaurantsQuery.Where(r => r.Location.BuildingName.Contains(building));
                }

                if (!string.IsNullOrEmpty(floor))
                {
                    restaurantsQuery = (IIncludableQueryable<Restaurant, Location>)restaurantsQuery.Where(r => r.Location.FloorNumber.Contains(floor));
                }

                if (!string.IsNullOrEmpty(menuItemName))
                {
                    menuItemsQuery = (IIncludableQueryable<MenuItem, Restaurant>)menuItemsQuery.Where(m => m.Name.Contains(menuItemName));
                }

                if (!string.IsNullOrEmpty(menuItemCategory))
                {
                    menuItemsQuery = (IIncludableQueryable<MenuItem, Restaurant>)menuItemsQuery.Where(m => m.Category.Contains(menuItemCategory));
                }

                if (menuItemRestaurant.HasValue)
                {
                    menuItemsQuery = (IIncludableQueryable<MenuItem, Restaurant>)menuItemsQuery.Where(m => m.RestaurantId == menuItemRestaurant.Value);
                }
            }
            catch
            {
                return RedirectToAction("Dashboard", "Admin");
            }

            

            // Fetch Active Orders
            var activeOrders = await _context.Orders
                .Include(o => o.User)
                .Where(o => o.OrderStatus == "Preparing" || o.OrderStatus == "Ready to pick" || o.OrderStatus == "Pending") // Adjust status as needed
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            var viewModel = new AdminDashboardViewModel
            {
                Restaurants = await restaurantsQuery.ToListAsync(),
                MenuItems = await menuItemsQuery.ToListAsync(),
                BuildingFilter = building,
                FloorFilter = floor,
                MenuItemNameFilter = menuItemName,
                MenuItemCategoryFilter = menuItemCategory,
                MenuItemRestaurantFilter = menuItemRestaurant,
                AllRestaurants = await _context.Restaurants.ToListAsync(), // For the Restaurant filter dropdown
                ActiveOrders = activeOrders // Assign the fetched active orders
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ActiveOrders(int? restaurantId)
        {
            var orders = _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        .ThenInclude(mi => mi.Restaurant)
                .Where(o => o.OrderStatus != "Completed");

            if (restaurantId.HasValue)
            {
                orders = orders.Where(o => o.OrderItems.Any(oi => oi.MenuItem.RestaurantId == restaurantId));
            }

            ViewBag.RestaurantId = restaurantId;
            ViewBag.Restaurants = new SelectList(await _context.Restaurants.ToListAsync(), "Id", "Name", restaurantId);

            return View(await orders.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                        .ThenInclude(mi => mi.Restaurant)
                .ToListAsync();
            return View(orders);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string orderStatus)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.OrderStatus = orderStatus;
                if (orderStatus == "Preparing" && !order.PreparationStartTime.HasValue)
                {
                    order.PreparationStartTime = DateTime.Now;
                }
                else if (orderStatus == "Ready to pick" && order.PreparationStartTime.HasValue && !order.PreparationEndTime.HasValue)
                {
                    order.PreparationEndTime = DateTime.Now;
                }
                else if (orderStatus == "Completed")
                {
                    order.PreparationEndTime = DateTime.Now; 
                }
                order.UpdatedOn = DateTime.Now;
                _context.Update(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("ActiveOrders"); 
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> IncreasePreparationTimer(int orderId, int additionalMinutes)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null && order.ScheduledTime.HasValue)
            {
                if (!order.PreparationEndTime.HasValue)
                {
                   
                    order.PreparationEndTime = order.PreparationEndTime?.AddMinutes(additionalMinutes) ?? DateTime.Now.AddMinutes(additionalMinutes);
                }
                else
                {
                    order.ScheduledTime = order.ScheduledTime?.AddMinutes(additionalMinutes);
                }
                order.PreparationTimeExtension = (order.PreparationTimeExtension ?? TimeSpan.Zero) + TimeSpan.FromMinutes(additionalMinutes);
                order.UpdatedOn = DateTime.Now;
                _context.Update(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("ActiveOrders");
            }
            return NotFound();
        }
    }
}