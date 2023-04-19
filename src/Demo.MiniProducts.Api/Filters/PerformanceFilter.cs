using System.Diagnostics;

namespace Demo.MiniProducts.Api.Filters;

public class PerformanceFilter<TRequest> : IEndpointFilter where TRequest : class
{
    private readonly ILogger _logger;

    public PerformanceFilter(ILoggerFactory loggerFactory) =>
        _logger = loggerFactory.CreateLogger(nameof(PerformanceFilter<TRequest>));

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var timer = new Stopwatch();
        timer.Start();
        _logger.LogInformation("Starting {@RequestType}", typeof(TRequest).Name);
        var response = await next(context);
        timer.Stop();
        _logger.LogInformation(
            "Ended {@RequestType}, time taken {@ProcessingTime}ms",
            typeof(TRequest).Name,
            timer.ElapsedMilliseconds
        );
        return response;
    }
}
