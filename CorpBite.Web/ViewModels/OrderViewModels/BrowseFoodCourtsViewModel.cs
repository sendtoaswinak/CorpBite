using CorpBite.Models;
using System.Collections.Generic;
using System.Linq;

namespace CorpBite.ViewModels.OrderViewModels
{
    public class BrowseFoodCourtsViewModel
    {
        public List<Location> Locations { get; set; }
        public int? SelectedLocationId { get; set; }
        public Dictionary<string, List<MenuItem>> MenuItemsByCategory { get; set; }
        public string SelectedLocationName { get; set; }
    }
}