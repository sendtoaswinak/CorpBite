﻿using CorpBite.Data;
using CorpBite.Models;
using CorpBite.ViewModels.OrderViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace CorpBite.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? locationId)
        {
            var model = new BrowseFoodCourtsViewModel();
            model.Locations = await _context.Locations.ToListAsync();
            model.SelectedLocationId = locationId;

            if (locationId.HasValue)
            {
                var selectedLocation = await _context.Locations.FindAsync(locationId);
                if (selectedLocation != null)
                {
                    model.SelectedLocationName = $"{selectedLocation.BuildingName} - Floor {selectedLocation.FloorNumber}";
                    var menuItems = await _context.MenuItems
                        .Where(m => m.Restaurant.LocationId == locationId && m.IsActive)
                        .Include(m => m.Restaurant)
                        .ToListAsync();

                    model.MenuItemsByCategory = menuItems
                        .GroupBy(m => m.Category)
                        .ToDictionary(g => g.Key, g => g.ToList());
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SetPreferredLocation(int locationId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userPreference = await _context.UserPreferences.FirstOrDefaultAsync(up => up.UserId == userId);

            if (userPreference == null)
            {
                userPreference = new UserPreference { UserId = userId, PreferredLocationId = locationId };
                _context.UserPreferences.Add(userPreference);
            }
            else
            {
                userPreference.PreferredLocationId = locationId;
                _context.UserPreferences.Update(userPreference);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Create", new { locationId = locationId });
        }

        [HttpGet]
        public async Task<IActionResult> BrowseByPreferredLocation()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userPreference = await _context.UserPreferences
                .Include(up => up.PreferredLocation)
                .FirstOrDefaultAsync(up => up.UserId == userId);

            if (userPreference?.PreferredLocationId != null)
            {
                return RedirectToAction("Create", new { locationId = userPreference.PreferredLocationId });
            }
            else
            {
                // Redirect to the main browse page if no preferred location is set
                return RedirectToAction("Create");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int menuItemId)
        {
            var menuItem = await _context.MenuItems.FindAsync(menuItemId);
            if (menuItem == null)
            {
                return NotFound();
            }

            var cart = GetCartFromSession();
            var existingItem = cart.FirstOrDefault(item => item.MenuItemId == menuItemId);

            if (existingItem != null)
            {
                existingItem.Quantity++;
            }
            else
            {
                cart.Add(new CartItemViewModel
                {
                    MenuItemId = menuItemId,
                    MenuItemName = menuItem.Name,
                    Price = menuItem.Price,
                    Quantity = 1
                });
            }

            SaveCartToSession(cart);
            return RedirectToAction("Create", new { locationId = menuItem.RestaurantId }); // Redirect back to the menu
        }

        [HttpGet]
        public IActionResult Cart()
        {
            var cart = GetCartFromSession();
            return View(cart);
        }

        [HttpPost]
        public IActionResult UpdateCartItem(int menuItemId, int quantity)
        {
            var cart = GetCartFromSession();
            var itemToUpdate = cart.FirstOrDefault(item => item.MenuItemId == menuItemId);

            if (itemToUpdate != null)
            {
                if (quantity > 0)
                {
                    itemToUpdate.Quantity = quantity;
                }
                else
                {
                    cart.Remove(itemToUpdate);
                }
                SaveCartToSession(cart);
            }
            return RedirectToAction("Cart");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int menuItemId)
        {
            var cart = GetCartFromSession();
            var itemToRemove = cart.FirstOrDefault(item => item.MenuItemId == menuItemId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                SaveCartToSession(cart);
            }
            return RedirectToAction("Cart");
        }

        [HttpGet]
        public IActionResult ScheduleOrder()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ScheduleOrder(DateTime scheduledTime)
        {
            var cart = GetCartFromSession();
            if (!cart.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return View();
            }

            if (scheduledTime <= DateTime.Now)
            {
                ModelState.AddModelError("ScheduledTime", "Scheduled time must be in the future.");
                return View();
            }

            return await ProcessOrder(cart, scheduledTime);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(string paymentMethod) // For immediate orders
        {
            var cart = GetCartFromSession();
            if (!cart.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return RedirectToAction("Cart");
            }

            // In a real application, you would integrate with a payment gateway here.
            // For this example, we'll just record the payment method.

            return await ProcessOrder(cart, null, paymentMethod);
        }

        private async Task<IActionResult> ProcessOrder(List<CartItemViewModel> cart, DateTime? scheduledTime = null, string paymentMethod = null)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                OrderStatus = scheduledTime.HasValue ? "Scheduled" : "Pending",
                TotalAmount = cart.Sum(item => item.Price * item.Quantity),
                PaymentMethod = paymentMethod,
                ScheduledTime = scheduledTime
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in cart)
            {
                var menuItem = await _context.MenuItems.FindAsync(item.MenuItemId);
                if (menuItem != null)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        MenuItemId = item.MenuItemId,
                        Quantity = item.Quantity,
                        Price = item.Price // Use the price at the time of ordering
                    };
                    _context.OrderItems.Add(orderItem);
                }
            }

            await _context.SaveChangesAsync();

            ClearCartSession();
            return RedirectToAction("Receipt", new { orderId = order.Id });
        }

        private List<CartItemViewModel> GetCartFromSession()
        {
            var sessionJson = HttpContext.Session.GetString("Cart");
            return sessionJson == null ? new List<CartItemViewModel>() : JsonSerializer.Deserialize<List<CartItemViewModel>>(sessionJson);
        }

        private void SaveCartToSession(List<CartItemViewModel> cart)
        {
            var sessionJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString("Cart", sessionJson);
        }

        private void ClearCartSession()
        {
            HttpContext.Session.Remove("Cart");
        }

        [HttpGet]
        public async Task<IActionResult> Receipt(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)));

            if (order == null)
            {
                return NotFound();
            }

            var orderViewModel = new OrderViewModel
            {
                OrderId = order.Id,
                OrderDate = order.OrderDate,
                OrderStatus = order.OrderStatus,
                TotalAmount = order.TotalAmount,
                PaymentMethod = order.PaymentMethod,
                ScheduledTime = order.ScheduledTime,
                OrderItems = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    MenuItemName = oi.MenuItem.Name,
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };

            return View(orderViewModel);
        }
    }
}
