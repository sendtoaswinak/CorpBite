using CorpBite.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly AppDbContext _context;

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Dashboard()
    {
        var today = DateTime.UtcNow.Date;
        var last30Days = DateTime.UtcNow.AddDays(-30).Date;

        var todayOrders = await _context.Orders
            .Where(o => o.OrderDate.Date == today)
            .ToListAsync();

        var viewModel = new AdminDashboardViewModel
        {
            TotalOrders = await _context.Orders.CountAsync(),
            PendingOrders = await _context.Orders
                .CountAsync(o => o.OrderStatus == "Pending" || o.OrderStatus == "Confirmed"),
            CompletedOrders = await _context.Orders
                .CountAsync(o => o.OrderStatus == "Completed"),
            TodayRevenue = todayOrders.Sum(o => o.TotalAmount),
            TotalFoodCourts = await _context.FoodCourts.CountAsync(),
            ActiveFoodCourts = await _context.FoodCourts.CountAsync(fc => fc.IsActive),
            TotalMenuItems = await _context.MenuItems.CountAsync(),
            ActiveMenuItems = await _context.MenuItems.CountAsync(mi => mi.IsActive && mi.IsAvailable),

            // Get order trends for the last 30 days
            OrderTrends = await _context.Orders
                .Where(o => o.OrderDate >= last30Days)
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new OrderTrend
                {
                    Date = g.Key,
                    OrderCount = g.Count(),
                    Revenue = g.Sum(o => o.TotalAmount)
                })
                .OrderBy(t => t.Date)
                .ToListAsync(),

            // Get top performing food courts
            TopFoodCourts = await _context.FoodCourts
                .Where(fc => fc.IsActive)
                .Select(fc => new TopFoodCourt
                {
                    Id = fc.Id,
                    Name = fc.Name,
                    OrderCount = fc.Orders.Count(o => o.OrderDate >= last30Days),
                    Revenue = fc.Orders
                        .Where(o => o.OrderDate >= last30Days)
                        .Sum(o => o.TotalAmount),
                    Rating = fc.Feedbacks.Average(f => (double?)f.Rating) ?? 0
                })
                .OrderByDescending(fc => fc.Revenue)
                .Take(5)
                .ToListAsync(),

            // Get top selling menu items
            TopMenuItems = await _context.MenuItems
                .Where(mi => mi.IsActive)
                .Select(mi => new TopMenuItem
                {
                    Id = mi.Id,
                    Name = mi.Name,
                    FoodCourtName = mi.FoodCourt.Name,
                    OrderCount = mi.OrderItems
                        .Count(oi => oi.Order.OrderDate >= last30Days),
                    Revenue = mi.OrderItems
                        .Where(oi => oi.Order.OrderDate >= last30Days)
                        .Sum(oi => oi.TotalPrice)
                })
                .OrderByDescending(mi => mi.OrderCount)
                .Take(5)
                .ToListAsync()
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Orders(string status = null,
        DateTime? fromDate = null, DateTime? toDate = null, string sortBy = "date_desc")
    {
        var query = _context.Orders
            .Include(o => o.FoodCourt)
            .Include(o => o.User)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.MenuItem)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(status))
            query = query.Where(o => o.OrderStatus == status);

        if (fromDate.HasValue)
            query = query.Where(o => o.OrderDate.Date >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(o => o.OrderDate.Date <= toDate.Value.Date);

        // Apply sorting
        query = sortBy switch
        {
            "date_asc" => query.OrderBy(o => o.OrderDate),
            "amount_desc" => query.OrderByDescending(o => o.TotalAmount),
            "amount_asc" => query.OrderBy(o => o.TotalAmount),
            _ => query.OrderByDescending(o => o.OrderDate)
        };

        var orders = await query.ToListAsync();

        return View(orders);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateOrderStatus(int id, string status)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
            return NotFound();

        order.OrderStatus = status;
        order.StatusHistory.Add(new OrderStatusHistory
        {
            Status = status,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = User.Identity.Name
        });

        await _context.SaveChangesAsync();

        return Json(new { success = true });
    }

    public async Task<IActionResult> FoodCourts()
    {
        var foodCourts = await _context.FoodCourts
            .Include(fc => fc.Location)
            .Include(fc => fc.MenuItems)
            .OrderBy(fc => fc.Location.BuildingName)
            .ThenBy(fc => fc.Location.FloorNumber)
            .ToListAsync();

        return View(foodCourts);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleFoodCourtStatus(int id)
    {
        var foodCourt = await _context.FoodCourts.FindAsync(id);
        if (foodCourt == null)
            return NotFound();

        foodCourt.IsActive = !foodCourt.IsActive;
        await _context.SaveChangesAsync();

        return Json(new { success = true, isActive = foodCourt.IsActive });
    }

    public async Task<IActionResult> MenuItems(int? foodCourtId = null)
    {
        var query = _context.MenuItems
            .Include(mi => mi.FoodCourt)
            .AsQueryable();

        if (foodCourtId.HasValue)
            query = query.Where(mi => mi.FoodCourtId == foodCourtId);

        var menuItems = await query
            .OrderBy(mi => mi.FoodCourt.Name)
            .ThenBy(mi => mi.Category)
            .ThenBy(mi => mi.Name)
            .ToListAsync();

        ViewBag.FoodCourts = await _context.FoodCourts
            .Where(fc => fc.IsActive)
            .OrderBy(fc => fc.Name)
            .ToListAsync();

        return View(menuItems);
    }

    [HttpPost]
    public async Task<IActionResult> ToggleMenuItemAvailability(int id)
    {
        var menuItem = await _context.MenuItems.FindAsync(id);
        if (menuItem == null)
            return NotFound();

        menuItem.IsAvailable = !menuItem.IsAvailable;
        await _context.SaveChangesAsync();

        return Json(new { success = true, isAvailable = menuItem.IsAvailable });
    }

    public async Task<IActionResult> Reports()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> GenerateReport(string type,
        DateTime fromDate, DateTime toDate)
    {
        // Implementation for generating different types of reports
        // This will be handled in the next part
        return Ok();
    }
}