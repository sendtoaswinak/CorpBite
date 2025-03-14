using Microsoft.AspNetCore.Identity;

namespace CorpBite.Web.Models
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string? Department { get; set; }
        public string? EmployeeId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<Order> Orders { get; set; } = new();
        public List<Feedback> Feedbacks { get; set; } = new();
        public List<FavoriteItem> FavoriteItems { get; set; } = new();
    }
}