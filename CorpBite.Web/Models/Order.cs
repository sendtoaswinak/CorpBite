using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CorpBite.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public string OrderStatus { get; set; } // e.g., Pending, Processing, Delivered, Cancelled

        [Required]
        public decimal TotalAmount { get; set; }

        public string PaymentMethod { get; set; }

        public DateTime? ScheduledTime { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; }
    }
}