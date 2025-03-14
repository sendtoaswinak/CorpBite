namespace CorpBite.Web.Models
{
    public class Location
    {
        public int Id { get; set; }
        public string BuildingName { get; set; }
        public int FloorNumber { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public List<FoodCourt> FoodCourts { get; set; } = new();
    }
}