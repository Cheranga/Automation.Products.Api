using Microsoft.EntityFrameworkCore;

namespace Demo.MiniProducts.Api.DataAccess;

public class ProductsDbContext : DbContext
{
    public ProductsDbContext(DbContextOptions<ProductsDbContext> options) : base(options)
    {
        
    }

    public DbSet<Product> Products => Set<Product>();
}