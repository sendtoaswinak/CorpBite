namespace CorpBite.Web.ViewModels.Order
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public string OrderNumber { get; set; }
        public string FoodCourtName { get; set; }
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
        public List<OrderItemViewModel> Items { get; set; } = new();
        public FeedbackViewModel? Feedback { get; set; }
    }

    public class OrderItemViewModel
    {
        public int MenuItemId { get; set; }
        public string Name { get; set; }
        public bool IsVeg { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? SpecialInstructions { get; set; }
    }
}