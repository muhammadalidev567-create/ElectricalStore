using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricalStore.Models;

public class Product
{
    public int ProductId { get; set; }

    [Required, StringLength(200)]
    public string ProductName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string SKU { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    [Required, StringLength(30)]
    public string Unit { get; set; } = "Piece";

    [Column(TypeName = "decimal(18,2)")]
    public decimal PurchasePrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal SalePrice { get; set; }

    public int MinimumStock { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Category? Category { get; set; }
    public ICollection<StockIn> StockIns { get; set; } = new List<StockIn>();
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
}
