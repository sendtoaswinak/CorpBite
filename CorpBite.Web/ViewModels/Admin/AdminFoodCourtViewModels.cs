using System.ComponentModel.DataAnnotations;

namespace CorpBite.ViewModels.Admin
{
    public class FoodCourtViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public int LocationId { get; set; }
        public string? ImageUrl { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Pure Veg")]
        public bool IsVeg { get; set; }

        [Display(Name = "Serves Non-Veg")]
        public bool IsNonVeg { get; set; }

        [Required(ErrorMessage = "Contact number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string ContactNumber { get; set; }

        [Required(ErrorMessage = "GST number is required")]
        [RegularExpression(@"^\d{2}[A-Z]{5}\d{4}[A-Z]{1}[A-Z\d]{1}[Z]{1}[A-Z\d]{1}$",
         ErrorMessage = "Invalid GST number")]
        public string GSTNumber { get; set; }

        [Required(ErrorMessage = "Opening time is required")]
        public TimeSpan OpeningTime { get; set; }

        [Required(ErrorMessage = "Closing time is required")]
        public TimeSpan ClosingTime { get; set; }

        public IFormFile? ImageFile { get; set; }
        public List<LocationViewModel> AvailableLocations { get; set; } = new();
    }
}