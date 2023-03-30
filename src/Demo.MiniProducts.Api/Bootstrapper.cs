using Demo.MiniProducts.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.MiniProducts.Api;

public static class Bootstrapper
{
    public static WebApplication Setup(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        RegisterDataAccess(builder);

        // TODO: add other dependencies
        return builder.Build();
    }

    private static void RegisterDataAccess(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ProductsDbContext>(
            optionsBuilder => optionsBuilder.UseInMemoryDatabase(nameof(ProductsDbContext))
        );
    }
}
