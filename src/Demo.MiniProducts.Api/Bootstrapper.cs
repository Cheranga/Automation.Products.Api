using Demo.MiniProducts.Api.DataAccess;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Demo.MiniProducts.Api;

public static class Bootstrapper
{
    public static WebApplication Setup(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        RegisterSwagger(builder);
        RegisterValidators(builder);
        RegisterDataAccess(builder);

        // TODO: add other dependencies
        return builder.Build();
    }

    private static void RegisterSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    private static void RegisterValidators(WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssembly(typeof(Bootstrapper).Assembly);
    }

    private static void RegisterDataAccess(WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ProductsDbContext>(
            optionsBuilder => optionsBuilder.UseInMemoryDatabase(nameof(ProductsDbContext))
        );
    }
}
