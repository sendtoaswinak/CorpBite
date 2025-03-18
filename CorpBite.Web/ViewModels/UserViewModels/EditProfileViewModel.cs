using System.ComponentModel.DataAnnotations;
using CorpBite.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CorpBite.ViewModels.UserViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Display(Name = "Location")]
        [Required(ErrorMessage = "Please select a location")]
        public int LocationId { get; set; }

        public List<SelectListItem> AvailableLocations { get; set; }
    }
}