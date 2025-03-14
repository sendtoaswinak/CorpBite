using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CorpBite.Models
{
    public class Restaurant
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string GSTNumber { get; set; }

        [Required]
        public int AdminUserId { get; set; }
        public User AdminUser { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }

        public ICollection<MenuItem> MenuItems { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
        public string Description { get; internal set; }
    }
}