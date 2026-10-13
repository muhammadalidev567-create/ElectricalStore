namespace ElectricalStore.Models;

public class InventoryRow
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public int TotalStock { get; set; }
    public int TotalSold { get; set; }
    public int AvailableStock { get; set; }
    public int MinimumStock { get; set; }
    public decimal SalePrice { get; set; }
}
