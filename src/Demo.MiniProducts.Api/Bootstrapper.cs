using Demo.MiniProducts.Api.Features.ChangeLocation;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentValidation;
using Funky.Azure.DataTable.Extensions.Core;
using Serilog;
using Storage.Queue.Helper;
using Swashbuckle.AspNetCore.Filters;
using Messages = Storage.Queue.Helper.Bootstrapper;

namespace Demo.MiniProducts.Api;

public static class Bootstrapper
{
    public static WebApplication Setup(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Logging.ClearProviders().AddJsonConsole();

        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });

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
        builder.Services.AddSwaggerGen(options =>
        {
            options.ExampleFilters();
        });
        builder.Services.AddSwaggerExamplesFromAssemblyOf(typeof(Bootstrapper));
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
