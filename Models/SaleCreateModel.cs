using System.ComponentModel.DataAnnotations;

namespace ElectricalStore.Models;

public class SaleCreateModel
{
    [StringLength(200)]
    public string? CustomerName { get; set; }

    [StringLength(50)]
    public string? CustomerPhone { get; set; }

    [StringLength(50)]
    public string PaymentMethod { get; set; } = "Cash";

    [Range(0, double.MaxValue)]
    public decimal Discount { get; set; }

    public string? Notes { get; set; }

    public List<SaleCreateItem> Items { get; set; } = new();
}

public class SaleCreateItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}
