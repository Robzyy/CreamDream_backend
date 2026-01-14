namespace CreamDream.DataTransferObjects.Orders;

public class OrderStatisticsResponse
{
    public int TotalOrders { get; set; }
    public int PendingOrders { get; set; }
    public int ProcessingOrders { get; set; }
    public int CompletedOrders { get; set; }
    public int CancelledOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageOrderValue { get; set; }
    public int TodayOrders { get; set; }
    public decimal WeekRevenue { get; set; }
}
