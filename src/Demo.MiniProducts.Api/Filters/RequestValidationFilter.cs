using Demo.MiniProducts.Api.Core;
using Demo.MiniProducts.Api.Extensions;
using FluentValidation;

namespace Demo.MiniProducts.Api.Filters;

public class RequestValidationFilter<TRequest, TValidator> : IEndpointFilter
    where TValidator : class, IValidator<TRequest>
    where TRequest : class, IValidatable<TRequest, TValidator>, new()
{
    private readonly ILogger _logger;
    private readonly IServiceProvider _serviceProvider;

    public RequestValidationFilter(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _logger = loggerFactory.CreateLogger(nameof(RequestValidationFilter<TRequest, TValidator>));
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        await using (var scope = _serviceProvider.CreateAsyncScope())
        {
            var validator = scope.ServiceProvider.GetRequiredService<TValidator>();

            if (!context.Arguments.Any())
                return await next(context);

            var findIndex =context.Arguments.ToList().FindIndex(x => x!.GetType() == typeof(TRequest));

            if (findIndex == -1)
                return await next(context);

            var request = context.GetArgument<TRequest>(findIndex);
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                _logger.LogError(
                    "Invalid request {@Request} with errors {@ValidationError}",
                    typeof(TRequest).Name,
                    validationResult.Errors
                );
                return validationResult.ToValidationErrorResponse();
            }
        }

        return await next(context);
    }
}
