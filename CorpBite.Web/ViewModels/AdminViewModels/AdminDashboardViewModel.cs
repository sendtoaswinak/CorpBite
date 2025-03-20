using CorpBite.Models;
using System.Collections.Generic;

namespace CorpBite.ViewModels.AdminViewModels
{
    public class AdminDashboardViewModel
    {
        public List<Restaurant> Restaurants { get; set; } 
        public List<MenuItem> MenuItems { get; set; }
        public List<Order> Orders { get; set; }
        public IEnumerable<Order> ActiveOrders { get; set; }
        public string BuildingFilter { get; set; }
        public string FloorFilter { get; set; }
        public string MenuItemNameFilter { get; set; }
        public string MenuItemCategoryFilter { get; set; }
        public int? MenuItemRestaurantFilter { get; set; } 
        public List<Restaurant> AllRestaurants { get; set; } 
    }
}