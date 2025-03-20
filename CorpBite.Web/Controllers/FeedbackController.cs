using CorpBite.Data;
using CorpBite.Models;
using CorpBite.ViewModels.FeedbackViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CorpBite.Controllers
{
    [Authorize]
    public class FeedbackController : Controller
    {
        private readonly AppDbContext _context;

        public FeedbackController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Submit(int? menuItemId)
        {
            var viewModel = new FeedbackViewModel();
            if (menuItemId.HasValue)
            {
                var menuItem = await _context.MenuItems.FindAsync(menuItemId);
                if (menuItem != null)
                {
                    viewModel.MenuItemId = menuItem.Id;
                    viewModel.MenuItemName = menuItem.Name;
                }
            }

           
            ViewBag.MenuItems = new SelectList(await _context.MenuItems.ToListAsync(), "Id", "Name", viewModel.MenuItemId);

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(FeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var feedback = new Review
                {
                    UserId = userId,
                    MenuItemId = model.MenuItemId,
                    Rating = model.Rating,
                    Comment = model.Comment
                };

                _context.Reviews.Add(feedback);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Thank you for your feedback!";
                return View(model);
            }

            ViewBag.MenuItems = new SelectList(await _context.MenuItems.ToListAsync(), "Id", "Name", model.MenuItemId);
            return View(model);
        }

        [Authorize(Roles = "Employee")]
        [HttpGet]
        public async Task<IActionResult> ViewEmployeeFeedback()
        {
            var employeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var feedback = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.MenuItem)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();

            return View(feedback);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> ViewAdminFeedback()
        {
            var feedback = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.MenuItem)
                .OrderByDescending(r => r.CreatedOn)
                .ToListAsync();

            return View(feedback);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ViewAdminFeedback");
        }
    }
}