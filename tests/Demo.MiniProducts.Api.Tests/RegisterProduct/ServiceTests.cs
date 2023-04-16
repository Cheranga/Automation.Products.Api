using Azure.Storage.Table.Wrapper.Commands;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Storage.Queue.Helper;

namespace Demo.MiniProducts.Api.Tests.RegisterProduct;

public static class ServiceTests
{
    private const string ProblemHeaderType = "application/problem+json";

    [Fact(DisplayName = "invalid request")]
    public static async Task InvalidRequest()
    {
        var validator = new Mock<IValidator<RegisterProductRequest>>();
        validator
            .Setup(
                x =>
                    x.ValidateAsync(
                        It.IsAny<RegisterProductRequest>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(
                new ValidationResult(new[] { new ValidationFailure("", "validation error") })
            );

        var op = await Service.RegisterProduct(
            It.IsAny<RegisterProductRequest>(),
            validator.Object,
            It.IsAny<RegisterProductSettings>(),
            It.IsAny<IQueueService>(),
            It.IsAny<ICommandService>(),
            It.IsAny<CancellationToken>()
        );

        var result = op.Result as ValidationProblem;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.ContentType.Should().Be(ProblemHeaderType);
        result.ProblemDetails.Title.Should().Be("Invalid Request");
        result.ProblemDetails.Type.Should().Be("InvalidRequest");
        result.ProblemDetails.Errors.Count.Should().Be(1);
    }

    [Fact(DisplayName = "cannot save product")]
    public static async Task CannotSaveProduct()
    {
        var validator = new Mock<IValidator<RegisterProductRequest>>();
        validator
            .Setup(
                x =>
                    x.ValidateAsync(
                        It.IsAny<RegisterProductRequest>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(new ValidationResult());
        var commandService = new Mock<ICommandService>();
        commandService
            .Setup(
                x =>
                    x.UpsertAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<ProductDataModel>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(CommandOperation.Fail(Error.New(500, "cannot save")));

        var op = await Service.RegisterProduct(
            new RegisterProductRequest("prod1", "laptop", "2010", "tech"),
            validator.Object,
            new RegisterProductSettings("test", "products", "products", "connection string"),
            It.IsAny<IQueueService>(),
            commandService.Object,
            It.IsAny<CancellationToken>()
        );

        var result = op.Result as ProblemHttpResult;
        result.Should().NotBeNull();
        result!.ContentType.Should().Be(ProblemHeaderType);
        result.ProblemDetails.Title.Should().Be("500");
        result.ProblemDetails.Type.Should().Be("Error");
        result.ProblemDetails.Detail.Should().Be("cannot save");
    }

    [Fact(DisplayName = "cannot publish product registered event")]
    public static async Task CannotPublishProductRegisteredEvent()
    {
        var validator = new Mock<IValidator<RegisterProductRequest>>();
        validator
            .Setup(
                x =>
                    x.ValidateAsync(
                        It.IsAny<RegisterProductRequest>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(new ValidationResult());
        var commandService = new Mock<ICommandService>();
        commandService
            .Setup(
                x =>
                    x.UpsertAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<ProductDataModel>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(CommandOperation.Success());

        var queueService = new Mock<IQueueService>();
        queueService
            .Setup(
                x =>
                    x.PublishAsync(
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>(),
                        It.IsAny<(string queue, Func<string> content)>()
                    )
            )
            .ReturnsAsync(QueueOperation.Failure(QueueOperationError.New(500, "cannot publish")));

        var op = await Service.RegisterProduct(
            new RegisterProductRequest("prod1", "laptop", "2010", "tech"),
            validator.Object,
            new RegisterProductSettings("test", "products", "products", "connection string"),
            queueService.Object,
            commandService.Object,
            It.IsAny<CancellationToken>()
        );

        var result = op.Result as ProblemHttpResult;
        result.Should().NotBeNull();
        result!.ContentType.Should().Be(ProblemHeaderType);
        result.ProblemDetails.Title.Should().Be("500");
        result.ProblemDetails.Type.Should().Be("Error");
        result.ProblemDetails.Detail.Should().Be("cannot publish");
    }
    
    [Fact(DisplayName = "successfully saved and event published")]
    public static async Task SuccessfullySavedAndEventPublished()
    {
        var validator = new Mock<IValidator<RegisterProductRequest>>();
        validator
            .Setup(
                x =>
                    x.ValidateAsync(
                        It.IsAny<RegisterProductRequest>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(new ValidationResult());
        var commandService = new Mock<ICommandService>();
        commandService
            .Setup(
                x =>
                    x.UpsertAsync(
                        It.IsAny<string>(),
                        It.IsAny<string>(),
                        It.IsAny<ProductDataModel>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(CommandOperation.Success());

        var queueService = new Mock<IQueueService>();
        queueService
            .Setup(
                x =>
                    x.PublishAsync(
                        It.IsAny<string>(),
                        It.IsAny<CancellationToken>(),
                        It.IsAny<(string queue, Func<string> content)>()
                    )
            )
            .ReturnsAsync(QueueOperation.Success);

        var op = await Service.RegisterProduct(
            new RegisterProductRequest("prod1", "laptop", "2010", "tech"),
            validator.Object,
            new RegisterProductSettings("test", "products", "products", "connection string"),
            queueService.Object,
            commandService.Object,
            It.IsAny<CancellationToken>()
        );

        var result = op.Result as Created;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(StatusCodes.Status201Created);
        result.Location.Should().Contain("/products/tech/prod1");
    }
}
