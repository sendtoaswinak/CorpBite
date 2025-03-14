namespace CorpBite.Web.Models
{
    public class FoodCourt
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; }
        public bool IsVeg { get; set; }
        public bool IsNonVeg { get; set; }
        public string ContactNumber { get; set; }
        public string GSTNumber { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public List<MenuItem> MenuItems { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
    }
}