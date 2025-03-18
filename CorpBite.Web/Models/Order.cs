using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CorpBite.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public string OrderStatus { get; set; } // e.g., Pending, Preparing, Ready to pick, Completed, Cancelled, Scheduled

        [Required]
        [Column(TypeName = "decimal(18,0)")]
        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime? ScheduledTime { get; set; }

        // New Properties for Order Preparation
        public DateTime? PreparationStartTime { get; set; }
        public DateTime? PreparationEndTime { get; set; }
        public TimeSpan? PreparationTimeExtension { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
    }
}