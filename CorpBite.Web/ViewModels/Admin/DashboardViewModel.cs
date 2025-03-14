public class AdminDashboardViewModel
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public decimal TodayRevenue { get; set; }
    public int TotalFoodCourts { get; set; }
    public int ActiveFoodCourts { get; set; }
    public int TotalMenuItems { get; set; }
    public int ActiveMenuItems { get; set; }
    public List<OrderTrend> OrderTrends { get; set; }
    public List<TopFoodCourt> TopFoodCourts { get; set; }
    public List<TopMenuItem> TopMenuItems { get; set; }
}

public class OrderTrend
{
    public DateTime Date { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}

public class TopFoodCourt
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
    public double Rating { get; set; }
}

public class TopMenuItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string FoodCourtName { get; set; }
    public int OrderCount { get; set; }
    public decimal Revenue { get; set; }
}