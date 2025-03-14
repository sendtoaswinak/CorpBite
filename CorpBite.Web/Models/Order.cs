namespace CorpBite.Web.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int FoodCourtId { get; set; }
        public FoodCourt FoodCourt { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? ScheduledTime { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string OrderStatus { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? CancellationReason { get; set; }
        public bool IsBulkOrder { get; set; }
        public string? SpecialInstructions { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public Feedback? Feedback { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }
}