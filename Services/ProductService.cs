using ElectricalStore.Data;
using ElectricalStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectricalStore.Services;

public class ProductService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public ProductService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<List<Product>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Products
            .Include(x => x.Category)
            .Where(x => x.IsActive)
            .OrderBy(x => x.ProductName)
            .ToListAsync();
    }

    public async Task<List<Category>> GetCategoriesAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Categories.Where(x => x.IsActive).OrderBy(x => x.CategoryName).ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        await using var db = await _factory.CreateDbContextAsync();
        product.CreatedAt = DateTime.Now;
        product.IsActive = true;
        db.Products.Add(product);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Product product)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var existing = await db.Products.FindAsync(product.ProductId);
        if (existing is null) return;

        existing.ProductName = product.ProductName;
        existing.SKU = product.SKU;
        existing.CategoryId = product.CategoryId;
        existing.Unit = product.Unit;
        existing.PurchasePrice = product.PurchasePrice;
        existing.SalePrice = product.SalePrice;
        existing.MinimumStock = product.MinimumStock;
        await db.SaveChangesAsync();
    }

    public async Task DeactivateAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var product = await db.Products.FindAsync(id);
        if (product is null) return;
        product.IsActive = false;
        await db.SaveChangesAsync();
    }
}
