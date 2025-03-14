using CorpBite.Web.Models.ViewModels.Order;

namespace CorpBite.ViewModels.Admin
{
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
        public List<OrderViewModel> LatestOrders { get; set; } = new();
        public List<TopFoodCourt> TopFoodCourts { get; set; } = new();
    }

    public class TopFoodCourt
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public double Rating { get; set; }
    }
}