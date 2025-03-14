using System.ComponentModel.DataAnnotations;

namespace CorpBite.ViewModels.RestaurantViewModels
{
    public class RestaurantViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        [Required]
        [Display(Name = "Location")]
        public int LocationId { get; set; }
        public string LocationName { get; set; } // For display
    }
}