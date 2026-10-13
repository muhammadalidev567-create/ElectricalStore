using ElectricalStore.Data;
using ElectricalStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectricalStore.Services;

public class StockService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public StockService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<List<Product>> GetProductsAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Products.Where(x => x.IsActive).OrderBy(x => x.ProductName).ToListAsync();
    }

    public async Task<List<StockIn>> GetHistoryAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.StockIns
            .Include(x => x.Product)
            .OrderByDescending(x => x.StockInDate)
            .ToListAsync();
    }

    public async Task AddAsync(StockIn stock)
    {
        await using var db = await _factory.CreateDbContextAsync();
        stock.StockInDate = stock.StockInDate == default ? DateTime.Now : stock.StockInDate;
        db.StockIns.Add(stock);
        await db.SaveChangesAsync();
    }

    public async Task<List<InventoryRow>> GetInventoryAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();

        var products = await db.Products
            .Include(x => x.Category)
            .Where(x => x.IsActive)
            .OrderBy(x => x.ProductName)
            .ToListAsync();

        var stock = await db.StockIns
            .GroupBy(x => x.ProductId)
            .Select(g => new { ProductId = g.Key, Qty = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.ProductId, x => x.Qty);

        var sold = await db.SaleItems
            .GroupBy(x => x.ProductId)
            .Select(g => new { ProductId = g.Key, Qty = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.ProductId, x => x.Qty);

        return products.Select(p =>
        {
            var totalStock = stock.GetValueOrDefault(p.ProductId);
            var totalSold = sold.GetValueOrDefault(p.ProductId);

            return new InventoryRow
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                SKU = p.SKU,
                CategoryName = p.Category?.CategoryName ?? "",
                Unit = p.Unit,
                TotalStock = totalStock,
                TotalSold = totalSold,
                AvailableStock = totalStock - totalSold,
                MinimumStock = p.MinimumStock,
                SalePrice = p.SalePrice
            };
        }).ToList();
    }
}
