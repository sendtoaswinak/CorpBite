using CorpBite.ViewModels.Order;

public class OrderHistoryViewModel
{
    public List<OrderViewModel> Orders { get; set; }
    public Dictionary<string, int> StatusCounts { get; set; }
    public OrderFilterModel Filter { get; set; }
}

public class OrderFilterModel
{
    public string Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string SortBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}