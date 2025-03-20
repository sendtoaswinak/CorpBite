using System.ComponentModel.DataAnnotations;

namespace CorpBite.ViewModels.FeedbackViewModels
{
    public class FeedbackViewModel
    {
        [Required]
        [Display(Name = "Menu Item")]
        public int MenuItemId { get; set; }

        public string MenuItemName { get; set; } 

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        public string Comment { get; set; }
    }
}