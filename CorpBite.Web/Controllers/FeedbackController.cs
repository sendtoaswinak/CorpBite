using CorpBite.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CorpBite.Web.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DateTime _currentDateTime = DateTime.Parse("2025-03-14 07:37:32");
        private readonly string _currentUser = "sendtoaswinak";

        public FeedbackController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrderFeedback(OrderFeedbackViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid feedback data" });

            var order = await _context.Orders
                .Include(o => o.Feedback)
                .FirstOrDefaultAsync(o => o.Id == model.OrderId && o.UserId == _currentUser);

            if (order == null)
                return Json(new { success = false, message = "Order not found" });

            if (order.Feedback != null)
                return Json(new { success = false, message = "Feedback already submitted" });

            var feedback = new Feedback
            {
                OrderId = order.Id,
                UserId = _currentUser,
                Rating = model.Rating,
                Comment = model.Comment,
                CreatedAt = _currentDateTime,
                CreatedBy = _currentUser
            };

            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            // Update order status if needed
            order.HasFeedback = true;
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Feedback submitted successfully" });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitItemFeedback(MenuItemFeedbackViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid feedback data" });

            var menuItem = await _context.MenuItems
                .FirstOrDefaultAsync(mi => mi.Id == model.MenuItemId);

            if (menuItem == null)
                return Json(new { success = false, message = "Menu item not found" });

            // Check if user already submitted feedback for this item
            var existingFeedback = await _context.Feedbacks
                .FirstOrDefaultAsync(f => f.MenuItemId == model.MenuItemId && f.UserId == _currentUser);

            if (existingFeedback != null)
            {
                existingFeedback.Rating = model.Rating;
                existingFeedback.Comment = model.Comment;
                existingFeedback.UpdatedAt = _currentDateTime;
                existingFeedback.UpdatedBy = _currentUser;
            }
            else
            {
                var feedback = new Feedback
                {
                    MenuItemId = model.MenuItemId,
                    UserId = _currentUser,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    CreatedAt = _currentDateTime,
                    CreatedBy = _currentUser
                };
                _context.Feedbacks.Add(feedback);
            }

            await _context.SaveChangesAsync();

            // Update menu item rating
            var averageRating = await _context.Feedbacks
                .Where(f => f.MenuItemId == model.MenuItemId)
                .AverageAsync(f => f.Rating);

            menuItem.Rating = averageRating;
            menuItem.ReviewCount = await _context.Feedbacks
                .CountAsync(f => f.MenuItemId == model.MenuItemId);

            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Feedback submitted successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetItemFeedbacks(int menuItemId, int page = 1)
        {
            int pageSize = 10;
            var feedbacks = await _context.Feedbacks
                .Include(f => f.User)
                .Where(f => f.MenuItemId == menuItemId)
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FeedbackViewModel
                {
                    UserName = f.User.Name,
                    Rating = f.Rating,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt
                })
                .ToListAsync();

            var totalCount = await _context.Feedbacks
                .CountAsync(f => f.MenuItemId == menuItemId);

            return Json(new
            {
                feedbacks,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                currentPage = page
            });
        }
    }
}