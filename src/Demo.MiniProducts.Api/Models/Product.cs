using Microsoft.EntityFrameworkCore;

namespace Demo.MiniProducts.Api.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string LocationCode { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Category { get; set; } = string.Empty;
    public bool IsOnPromotion { get; set; }
}

public class ProductsDbContext : DbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
    {
        
    }

    public DbSet<Product> Products => Set<Product>();
}