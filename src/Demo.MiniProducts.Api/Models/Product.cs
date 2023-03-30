using Microsoft.EntityFrameworkCore;

namespace Demo.MiniProducts.Api.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsComplete { get; set; }
}

public class ProductsDbContext : DbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
    {
        
    }

    public DbSet<Product> Products => Set<Product>();
}