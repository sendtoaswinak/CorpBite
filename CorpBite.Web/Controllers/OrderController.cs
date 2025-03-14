using CorpBite.Web.Models;
using CorpBite.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CorpBite.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DateTime _currentDateTime = DateTime.Parse("2025-03-14 07:37:32");
        private readonly string _currentUser = "sendtoaswinak";

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.FoodCourt)
                .Where(o => o.UserId == _currentUser)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderListItemViewModel
                {
                    Id = o.Id,
                    OrderNumber = o.OrderNumber,
                    FoodCourtName = o.FoodCourt.Name,
                    OrderDate = o.OrderDate,
                    ScheduledTime = o.ScheduledTime,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    IsBulkOrder = o.IsBulkOrder
                })
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.FoodCourt)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.MenuItem)
                .Include(o => o.Feedback)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == _currentUser);

            if (order == null)
                return NotFound();

            var viewModel = new OrderDetailsViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                FoodCourtName = order.FoodCourt.Name,
                OrderDate = order.OrderDate,
                ScheduledTime = order.ScheduledTime,
                SubTotal = order.SubTotal,
                TaxAmount = order.TaxAmount,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                IsBulkOrder = order.IsBulkOrder,
                SpecialInstructions = order.SpecialInstructions,
                Items = order.OrderItems.Select(oi => new OrderItemViewModel
                {
                    MenuItemId = oi.MenuItemId,
                    Name = oi.MenuItem.Name,
                    IsVeg = oi.MenuItem.IsVeg,
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice,
                    TotalPrice = oi.TotalPrice,
                    SpecialInstructions = oi.SpecialInstructions
                }).ToList(),
                Feedback = order.Feedback != null ? new FeedbackViewModel
                {
                    Rating = order.Feedback.Rating,
                    Comment = order.Feedback.Comment,
                    CreatedAt = order.Feedback.CreatedAt
                } : null
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == _currentUser);

            if (order == null)
                return NotFound();

            if (order.OrderStatus != "Pending" && order.OrderStatus != "Confirmed")
                return BadRequest("Order cannot be cancelled in its current status");

            order.OrderStatus = "Cancelled";
            order.UpdatedAt = _currentDateTime;
            order.UpdatedBy = _currentUser;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = order.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                        .ThenInclude(ci => ci.MenuItem)
                    .FirstOrDefaultAsync(c => c.UserId == _currentUser);

                if (cart == null || !cart.Items.Any())
                    return RedirectToAction("Index", "Cart");

                var order = new Order
                {
                    UserId = _currentUser,
                    FoodCourtId = cart.FoodCourtId,
                    OrderNumber = GenerateOrderNumber(),
                    OrderDate = _currentDateTime,
                    ScheduledTime = model.ScheduledTime,
                    OrderStatus = "Pending",
                    IsBulkOrder = model.IsBulkOrder,
                    SpecialInstructions = model.SpecialInstructions,
                    CreatedAt = _currentDateTime,
                    CreatedBy = _currentUser
                };

                foreach (var cartItem in cart.Items)
                {
                    var orderItem = new OrderItem
                    {
                        MenuItemId = cartItem.MenuItemId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.MenuItem.Price,
                        TotalPrice = cartItem.MenuItem.Price * cartItem.Quantity,
                        SpecialInstructions = cartItem.SpecialInstructions,
                        CreatedAt = _currentDateTime,
                        CreatedBy = _currentUser
                    };
                    order.OrderItems.Add(orderItem);
                }

                order.SubTotal = order.OrderItems.Sum(oi => oi.TotalPrice);
                order.TaxAmount = Math.Round(order.SubTotal * 0.05m, 2); // 5% tax
                order.TotalAmount = order.SubTotal + order.TaxAmount;

                _context.Orders.Add(order);
                _context.Carts.Remove(cart);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private string GenerateOrderNumber()
        {
            return $"ORD{_currentDateTime:yyyyMMddHHmmss}{Random.Shared.Next(1000, 9999)}";
        }
    }
}