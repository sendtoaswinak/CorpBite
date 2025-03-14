namespace CorpBite.ViewModels.RestaurantViewModels
{
    public class MenuItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }
        public bool IsActive { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }
    }
}