using System;
using System.ComponentModel.DataAnnotations;

namespace CorpBite.Models
{
    public class UserPreference
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        public int? PreferredLocationId { get; set; }
        public Location PreferredLocation { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
    }
}