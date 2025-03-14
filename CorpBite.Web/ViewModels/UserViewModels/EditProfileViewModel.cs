using System.ComponentModel.DataAnnotations;

namespace CorpBite.ViewModels.UserViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
    }
}