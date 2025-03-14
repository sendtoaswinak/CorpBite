namespace CorpBite.Web.Models
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int FoodCourtId { get; set; }
        public FoodCourt FoodCourt { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsVeg { get; set; }
        public bool IsActive { get; set; }
        public bool IsAvailable { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public List<CartItem> CartItems { get; set; } = new();
        public List<Feedback> Feedbacks { get; set; } = new();
        public List<FavoriteItem> FavoriteItems { get; set; } = new();
    }
}