using ElectricalStore.Data;
using ElectricalStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectricalStore.Services;

public class ReportService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public ReportService(IDbContextFactory<ApplicationDbContext> factory)
    {
        _factory = factory;
    }

    // =========================================================
    // INVENTORY REPORT
    // =========================================================
    public async Task<List<InventoryRow>> InventoryAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();

        // Get active products
        var products = await db.Products
            .Include(x => x.Category)
            .Where(x => x.IsActive)
            .OrderBy(x => x.ProductName)
            .ToListAsync();

        // Total stock received for each product
        var stock = await db.StockIns
            .GroupBy(x => x.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToDictionaryAsync(
                x => x.ProductId,
                x => x.Quantity);

        // Total sold quantity for each product
        var sold = await db.SaleItems
            .GroupBy(x => x.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToDictionaryAsync(
                x => x.ProductId,
                x => x.Quantity);

        // Create inventory report
        return products
            .Select(product =>
            {
                var totalStock =
                    stock.GetValueOrDefault(product.ProductId);

                var totalSold =
                    sold.GetValueOrDefault(product.ProductId);

                var availableStock =
                    totalStock - totalSold;

                return new InventoryRow
                {
                    ProductId = product.ProductId,

                    ProductName = product.ProductName,

                    SKU = product.SKU,

                    CategoryName =
                        product.Category?.CategoryName ?? "",

                    Unit = product.Unit,

                    TotalStock = totalStock,

                    TotalSold = totalSold,

                    AvailableStock = availableStock,

                    MinimumStock = product.MinimumStock,

                    SalePrice = product.SalePrice
                };
            })
            .OrderBy(x => x.ProductName)
            .ToList();
    }


    // =========================================================
    // SALES REPORT
    // =========================================================
    public async Task<List<Sale>> SalesAsync(
        DateTime? from,
        DateTime? to)
    {
        await using var db = await _factory.CreateDbContextAsync();

        var query = db.Sales
            .AsNoTracking()
            .AsQueryable();


        // From Date
        if (from.HasValue)
        {
            var fromDate = from.Value.Date;

            query = query.Where(x =>
                x.SaleDate >= fromDate);
        }


        // To Date
        if (to.HasValue)
        {
            // Next day at 00:00
            // so the complete selected date is included
            var toDate = to.Value.Date.AddDays(1);

            query = query.Where(x =>
                x.SaleDate < toDate);
        }


        return await query
            .OrderByDescending(x => x.SaleDate)
            .ToListAsync();
    }
}