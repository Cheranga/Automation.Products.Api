using Demo.MiniProducts.Api.Features.ChangeLocation;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentValidation;
using Storage.Queue.Helper;
using Storage.Table.Helper;
using Tables = Storage.Table.Helper.Bootstrapper;
using Messages = Storage.Queue.Helper.Bootstrapper;

namespace Demo.MiniProducts.Api;

public static class Bootstrapper
{
    public static WebApplication Setup(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        RegisterSwagger(builder);
        RegisterSettings(builder);
        RegisterValidators(builder);
        RegisterDataAccess(builder);
        RegisterMessaging(builder);

        return builder.Build();
    }

    private static void RegisterSettings(WebApplicationBuilder builder)
    {
        var registerSettings = builder.Configuration
            .GetSection(nameof(RegisterProductSettings))
            .Get<RegisterProductSettings>();

        var updateSettings = builder.Configuration
            .GetSection(nameof(UpdateProductSettings))
            .Get<UpdateProductSettings>();

        builder.Services.AddSingleton(registerSettings!);
        builder.Services.AddSingleton(updateSettings!);
    }

    private static void RegisterMessaging(WebApplicationBuilder builder)
    {
        var registerSettings = builder.Configuration
            .GetSection(nameof(RegisterProductSettings))
            .Get<RegisterProductSettings>();
        builder.Services.RegisterMessagingWithConnectionString(
            registerSettings!.Category,
            registerSettings.ConnectionString
        );

        var updateSettings = builder.Configuration
            .GetSection(nameof(UpdateProductSettings))
            .Get<UpdateProductSettings>();

        builder.Services.RegisterMessagingWithConnectionString(
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

    private static void RegisterDataAccess(WebApplicationBuilder builder)
    {
        var settings = builder.Configuration
            .GetSection(nameof(RegisterProductSettings))
            .Get<RegisterProductSettings>();
        builder.Services.RegisterTablesWithConnectionString(
            settings!.Category,
            settings.ConnectionString
        );
    }
}
