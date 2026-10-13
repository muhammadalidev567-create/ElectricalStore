namespace ElectricalStore.Models;

public class DashboardModel
{
    public int TotalProducts { get; set; }
    public int TotalStock { get; set; }
    public int TotalSold { get; set; }
    public int AvailableStock { get; set; }
    public decimal TotalSales { get; set; }
    public int LowStockProducts { get; set; }
}
