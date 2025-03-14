namespace CorpBite.Web.ViewModels.Profile
{
    public class UserProfileViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string EmployeeId { get; set; }
        public string Department { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<OrderViewModel> RecentOrders { get; set; } = new();
        public List<FavoriteItemViewModel> FavoriteItems { get; set; } = new();
    }

    public class FavoriteItemViewModel
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsVeg { get; set; }
        public bool IsAvailable { get; set; }
        public string FoodCourtName { get; set; }
    }
}