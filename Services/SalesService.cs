using ElectricalStore.Data;
using ElectricalStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectricalStore.Services;

public class SalesService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public SalesService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<List<Product>> GetProductsAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Products.Where(x => x.IsActive).OrderBy(x => x.ProductName).ToListAsync();
    }

    public async Task<List<Sale>> GetSalesAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Sales.OrderByDescending(x => x.SaleDate).Take(200).ToListAsync();
    }

    public async Task<string> CreateSaleAsync(SaleCreateModel model)
    {
        if (model.Items.Count == 0)
            throw new InvalidOperationException("Sale mein kam az kam 1 product hona chahiye.");

        await using var db = await _factory.CreateDbContextAsync();
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var grouped = model.Items
                .Where(x => x.ProductId > 0 && x.Quantity > 0)
                .GroupBy(x => x.ProductId)
                .Select(g => new SaleCreateItem
                {
                    ProductId = g.Key,
                    Quantity = g.Sum(x => x.Quantity),
                    UnitPrice = g.First().UnitPrice
                }).ToList();

            if (grouped.Count == 0)
                throw new InvalidOperationException("Valid sale items nahi hain.");

            var productIds = grouped.Select(x => x.ProductId).ToList();
            var products = await db.Products
                .Where(x => productIds.Contains(x.ProductId) && x.IsActive)
                .ToDictionaryAsync(x => x.ProductId);

            decimal total = 0;
            var saleItems = new List<SaleItem>();

            foreach (var item in grouped)
            {
                if (!products.TryGetValue(item.ProductId, out var product))
                    throw new InvalidOperationException("Product nahi mila.");

                var stockIn = await db.StockIns
                    .Where(x => x.ProductId == item.ProductId)
                    .SumAsync(x => (int?)x.Quantity) ?? 0;

                var sold = await db.SaleItems
                    .Where(x => x.ProductId == item.ProductId)
                    .SumAsync(x => (int?)x.Quantity) ?? 0;

                var available = stockIn - sold;

                if (item.Quantity > available)
                    throw new InvalidOperationException(
                        $"{product.ProductName} ka available stock {available} hai.");

                var price = item.UnitPrice > 0 ? item.UnitPrice : product.SalePrice;
                total += item.Quantity * price;

                saleItems.Add(new SaleItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = price
                });
            }

            if (model.Discount < 0 || model.Discount > total)
                throw new InvalidOperationException("Discount valid nahi hai.");

            var sale = new Sale
            {
                InvoiceNo = $"INV-{DateTime.Now:yyyyMMddHHmmssfff}",
                CustomerName = model.CustomerName,
                CustomerPhone = model.CustomerPhone,
                SaleDate = DateTime.Now,
                TotalAmount = total,
                Discount = model.Discount,
                NetAmount = total - model.Discount,
                PaymentMethod = model.PaymentMethod,
                Notes = model.Notes,
                SaleItems = saleItems
            };

            db.Sales.Add(sale);
            await db.SaveChangesAsync();
            await transaction.CommitAsync();

            return sale.InvoiceNo;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
