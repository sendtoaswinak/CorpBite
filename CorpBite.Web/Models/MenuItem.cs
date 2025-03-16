using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorpBite.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Required]
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]

        [Column(TypeName = "decimal(18,0)")]
        public decimal Price { get; set; }

        public string Category { get; set; } // e.g., Veg, Non-Veg, Drinks

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
    }
}