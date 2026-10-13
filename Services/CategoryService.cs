using ElectricalStore.Data;
using ElectricalStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectricalStore.Services;

public class CategoryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory;

    public CategoryService(IDbContextFactory<ApplicationDbContext> factory) => _factory = factory;

    public async Task<List<Category>> GetAllAsync()
    {
        await using var db = await _factory.CreateDbContextAsync();
        return await db.Categories
            .Include(x => x.Products)
            .OrderBy(x => x.CategoryName)
            .ToListAsync();
    }

    public async Task AddAsync(Category category)
    {
        await using var db = await _factory.CreateDbContextAsync();
        category.CreatedAt = DateTime.Now;
        category.IsActive = true;
        db.Categories.Add(category);
        await db.SaveChangesAsync();
    }

    public async Task UpdateAsync(Category category)
    {
        await using var db = await _factory.CreateDbContextAsync();
        db.Categories.Update(category);
        await db.SaveChangesAsync();
    }

    public async Task DeactivateAsync(int id)
    {
        await using var db = await _factory.CreateDbContextAsync();
        var category = await db.Categories.FindAsync(id);
        if (category is null) return;
        category.IsActive = false;
        await db.SaveChangesAsync();
    }
}
