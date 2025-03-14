using Microsoft.AspNetCore.Mvc.Rendering;

namespace CorpBite.ViewModels.Cart
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal TaxRate { get; set; } = 0.05m; // 5% tax
        public decimal TaxAmount => SubTotal * TaxRate;
        public decimal Total => SubTotal + TaxAmount;
        public List<SelectListItem> AvailablePickupTimes { get; set; } = new();
    }

    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public string FoodCourtName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool IsVeg { get; set; }
        public string ImageUrl { get; set; }
        public string? SpecialInstructions { get; set; }
    }
}