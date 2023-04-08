using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Features.ChangeLocation;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Storage.Queue.Helper;

namespace Demo.MiniProducts.Api;

public static class Bootstrapper
{
    public static WebApplication Setup(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        RegisterSwagger(builder);
        RegisterValidators(builder);
        RegisterDataAccess(builder);
        RegisterMessaging(builder);

        return builder.Build();
    }

    private static void RegisterMessaging(WebApplicationBuilder builder)
    {
        var registerSettings = builder.Configuration
            .GetSection(nameof(RegisterProductSettings))
            .Get<RegisterProductSettings>();
        builder.Services.AddSingleton(registerSettings!);
        builder.Services.RegisterWithConnectionString(
            registerSettings!.Category,
            registerSettings.ConnectionString
        );

        var updateSettings = builder.Configuration
            .GetSection(nameof(UpdateProductSettings))
            .Get<UpdateProductSettings>();
        builder.Services.AddSingleton(updateSettings!);
        builder.Services.RegisterWithConnectionString(
            updateSettings!.Category,
            updateSettings.ConnectionString
        );
    }

    private static void RegisterSwagger(WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
    }

    private static void RegisterValidators(WebApplicationBuilder builder) =>
        builder.Services.AddValidatorsFromAssembly(typeof(Bootstrapper).Assembly);

    private static void RegisterDataAccess(WebApplicationBuilder builder) =>
        builder.Services.AddDbContext<ProductsDbContext>(
            optionsBuilder => optionsBuilder.UseInMemoryDatabase(nameof(ProductsDbContext))
        );
}
