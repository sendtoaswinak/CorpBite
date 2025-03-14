using System.ComponentModel.DataAnnotations;

namespace CorpBite.Web.ViewModels.Admin
{
    public class MenuItemViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Food Court is required")]
        public int FoodCourtId { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Is Vegetarian")]
        public bool IsVeg { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        public List<string> Categories { get; set; } = new();
        public List<FoodCourtViewModel> AvailableFoodCourts { get; set; } = new();
    }
}