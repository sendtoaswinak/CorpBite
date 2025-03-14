﻿using System;
using System.ComponentModel.DataAnnotations;

namespace CorpBite.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Role { get; set; } = "Employee"; // Default role
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedOn { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public int? LocationId { get; set; }
        public Location Location { get; set; }
    }
}