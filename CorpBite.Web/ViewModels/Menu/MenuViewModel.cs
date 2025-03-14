namespace CorpBite.ViewModels.Menu
{
    public class MenuViewModel
    {
        public int FoodCourtId { get; set; }
        public string FoodCourtName { get; set; }
        public List<MenuCategoryViewModel> Categories { get; set; } = new();
        public List<MenuItemViewModel> Items { get; set; } = new();
    }

    public class MenuCategoryViewModel
    {
        public string Name { get; set; }
        public int ItemCount { get; set; }
        public bool IsVeg { get; set; }
    }

    public class MenuItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsVeg { get; set; }
        public bool IsAvailable { get; set; }
        public string Category { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class MenuItemBasicViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public bool IsVeg { get; set; }
        public double Rating { get; set; }
    }
}