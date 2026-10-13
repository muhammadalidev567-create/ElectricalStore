using ElectricalStore.Data;
using ElectricalStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectricalStore.Services;

public class DashboardService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public DashboardService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<DashboardModel> GetAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();

        var totalStock = await db.StockIns.SumAsync(x => (int?)x.Quantity) ?? 0;
        var totalSold = await db.SaleItems.SumAsync(x => (int?)x.Quantity) ?? 0;
        var totalSales = await db.Sales.SumAsync(x => (decimal?)x.NetAmount) ?? 0m;

        var inventory = await (
            from p in db.Products
            where p.IsActive
            let stock = db.StockIns.Where(s => s.ProductId == p.ProductId).Sum(s => (int?)s.Quantity) ?? 0
            let sold = db.SaleItems.Where(s => s.ProductId == p.ProductId).Sum(s => (int?)s.Quantity) ?? 0
            select new { stock, sold, p.MinimumStock }
        ).ToListAsync();

        return new DashboardModel
        {
            TotalProducts = await db.Products.CountAsync(x => x.IsActive),
            TotalStock = totalStock,
            TotalSold = totalSold,
            AvailableStock = totalStock - totalSold,
            TotalSales = totalSales,
            LowStockProducts = inventory.Count(x => x.stock - x.sold <= x.MinimumStock)
        };
    }
}
