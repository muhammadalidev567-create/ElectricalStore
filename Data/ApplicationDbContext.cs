using ElectricalStore.Models;
using Microsoft.EntityFrameworkCore;

namespace ElectricalStore.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<StockIn> StockIns => Set<StockIn>();
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Category>()
            .HasKey(x => x.CategoryId);

        modelBuilder.Entity<Product>()
            .HasKey(x => x.ProductId);

        modelBuilder.Entity<Product>()
            .HasIndex(x => x.SKU)
            .IsUnique();

        modelBuilder.Entity<StockIn>()
            .HasKey(x => x.StockInId);

        modelBuilder.Entity<StockIn>()
            .ToTable("StockIn");

        modelBuilder.Entity<Sale>()
            .HasKey(x => x.SaleId);

        modelBuilder.Entity<Sale>()
            .HasIndex(x => x.InvoiceNo)
            .IsUnique();

        modelBuilder.Entity<SaleItem>()
            .HasKey(x => x.SaleItemId);

        modelBuilder.Entity<Category>()
            .HasMany(x => x.Products)
            .WithOne(x => x.Category)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasMany(x => x.StockIns)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Product>()
            .HasMany(x => x.SaleItems)
            .WithOne(x => x.Product)
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Sale>()
            .HasMany(x => x.SaleItems)
            .WithOne(x => x.Sale)
            .HasForeignKey(x => x.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Product>().Property(x => x.PurchasePrice).HasPrecision(18, 2);
        modelBuilder.Entity<Product>().Property(x => x.SalePrice).HasPrecision(18, 2);
        modelBuilder.Entity<StockIn>().Property(x => x.PurchasePrice).HasPrecision(18, 2);
        modelBuilder.Entity<Sale>().Property(x => x.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Sale>().Property(x => x.Discount).HasPrecision(18, 2);
        modelBuilder.Entity<Sale>().Property(x => x.NetAmount).HasPrecision(18, 2);
        modelBuilder.Entity<SaleItem>().Property(x => x.UnitPrice).HasPrecision(18, 2);
    }
}
