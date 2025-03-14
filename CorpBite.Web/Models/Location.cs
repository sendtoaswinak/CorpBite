using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CorpBite.Models
{
    public class Location
    {
        public int Id { get; set; }

        [Required]
        public string BuildingName { get; set; }

        [Required]
        public string FloorNumber { get; set; }

        public ICollection<User> Users { get; set; }
        public ICollection<Restaurant> Restaurants { get; set; }
        public ICollection<UserPreference> UserPreferences { get; set; }
    }
}