using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalStore.Models;

public class StockIn
{
    public int StockInId { get; set; }
    public int ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; }

    [StringLength(200)]
    public string? SupplierName { get; set; }

    [StringLength(100)]
    public string? ReferenceNo { get; set; }

    public DateTime StockInDate { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public Product? Product { get; set; }
}
