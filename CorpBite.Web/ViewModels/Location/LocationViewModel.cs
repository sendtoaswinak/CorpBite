using CorpBite.ViewModels.Menu;

namespace CorpBite.ViewModels.Location
{
    public class LocationViewModel
    {
        public int Id { get; set; }
        public string BuildingName { get; set; }
        public int FloorNumber { get; set; }
        public string Description { get; set; }
        public List<FoodCourtBasicViewModel> FoodCourts { get; set; } = new();
    }

    public class LocationDetailsViewModel : LocationViewModel
    {
        public List<FoodCourtDetailViewModel> FoodCourts { get; set; } = new();
    }

    public class FoodCourtBasicViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public bool IsVeg { get; set; }
        public bool IsNonVeg { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
    }

    public class FoodCourtDetailViewModel : FoodCourtBasicViewModel
    {
        public string Description { get; set; }
        public string ContactNumber { get; set; }
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public List<MenuItemBasicViewModel> PopularItems { get; set; } = new();
    }
}