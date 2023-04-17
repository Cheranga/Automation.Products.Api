using Azure.Storage.Table.Wrapper.Commands;
using Azure.Storage.Table.Wrapper.Queries;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Features.ChangeLocation;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Storage.Queue.Helper;
using Service = Demo.MiniProducts.Api.Features.ChangeLocation.Service;
using CR = Microsoft.AspNetCore.Http.HttpResults.Results<
    Microsoft.AspNetCore.Http.HttpResults.ValidationProblem,
    Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult,
    Microsoft.AspNetCore.Http.HttpResults.NotFound,
    Microsoft.AspNetCore.Http.HttpResults.NoContent
>;

namespace Demo.MiniProducts.Api.Tests.ChangeLocation;

public static class ServiceTests
{
    private const string ProblemHeaderType = "application/problem+json";

    [Fact(DisplayName = "Invalid location change request")]
    public static async Task InvalidRequest()
    {
        var validator = new Mock<IValidator<ChangeLocationRequest>>();
        validator
            .Setup(
                x =>
                    x.ValidateAsync(
                        It.IsAny<ChangeLocationRequest>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(
                new ValidationResult(new[] { new ValidationFailure("", "validation error") })
            );

        var response = await Service.ChangeLocation(
            It.IsAny<ChangeLocationRequest>(),
            validator.Object,
            It.IsAny<UpdateProductSettings>(),
            It.IsAny<RegisterProductSettings>(),
            It.IsAny<IQueueService>(),
            It.IsAny<IQueryService>(),
            It.IsAny<ICommandService>(),
            It.IsAny<CancellationToken>()
        );

        var result = response.Result as ValidationProblem;
        result.Should().NotBeNull();
        result!.ContentType.Should().Be(ProblemHeaderType);
        result.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        result.ProblemDetails.Errors.Count.Should().Be(1);
    }

    [Fact(DisplayName = "Product does not exist")]
    public static async Task ProductDoesNotExist()
    {
        var validator = new Mock<IValidator<ChangeLocationRequest>>();
        validator
            .Setup(
                x =>
                    x.ValidateAsync(
                        It.IsAny<ChangeLocationRequest>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(new ValidationResult());

        var queryService = new Mock<IQueryService>();
        queryService
            .Setup(
                x =>
                    x.GetEntityAsync<ProductDataModel>(
                        "test",
                        "products",
                        "TECH",
                        "PROD1",
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(QueryResult.Empty());

        var response = await Service.ChangeLocation(
            new ChangeLocationRequest
            {
                Category = "tech",
                Id = "prod1",
                LocationCode = "2020"
            },
            validator.Object,
            new UpdateProductSettings("test", "products", "connection string"),
            new RegisterProductSettings("test", "products", "products", "connection string"),
            It.IsAny<IQueueService>(),
            queryService.Object,
            It.IsAny<ICommandService>(),
            It.IsAny<CancellationToken>()
        );

        var result = response.Result as NotFound;
        result.Should().NotBeNull();
    }

    [Fact(DisplayName = "Error when saving updated product")]
    public static async Task ErrorWhenSavingProduct()
    {
        var validator = new Mock<IValidator<ChangeLocationRequest>>();
        validator
            .Setup(
                x =>
                    x.ValidateAsync(
                        It.IsAny<ChangeLocationRequest>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(new ValidationResult());

        var queryService = new Mock<IQueryService>();
        queryService
            .Setup(
                x =>
                    x.GetEntityAsync<ProductDataModel>(
                        "test",
                        "products",
                        "TECH",
                        "PROD1",
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(
                QueryResult.Single(ProductDataModel.New("tech", "prod1", "laptop", "2010"))
            );

        var commandService = new Mock<ICommandService>();
        commandService
            .Setup(
                x =>
                    x.UpdateAsync(
                        "test",
                        "products",
                        It.IsAny<ProductDataModel>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(CommandOperation.Fail(Error.New(1, "error")));

        var response = await Service.ChangeLocation(
            new ChangeLocationRequest
            {
                Category = "tech",
                Id = "prod1",
                LocationCode = "2020"
            },
            validator.Object,
            new UpdateProductSettings("test", "products", "connection string"),
            new RegisterProductSettings("test", "products", "products", "connection string"),
            It.IsAny<IQueueService>(),
            queryService.Object,
            commandService.Object,
            It.IsAny<CancellationToken>()
        );

        var result = response.Result as ProblemHttpResult;
        result.Should().NotBeNull();
        result!.ContentType.Should().Be(ProblemHeaderType);
        result.ProblemDetails.Title.Should().Be("1");
        result.ProblemDetails.Type.Should().Be("Error");
        result.ProblemDetails.Detail.Should().Be("error");
    }

    [Fact(DisplayName = "Error when publishing product updated event")]
    public static async Task ErrorWhenPublishingEvent()
    {
        var validator = new Mock<IValidator<ChangeLocationRequest>>();
        validator
            .Setup(
                x =>
                    x.ValidateAsync(
                        It.IsAny<ChangeLocationRequest>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(new ValidationResult());

        var queryService = new Mock<IQueryService>();
        queryService
            .Setup(
                x =>
                    x.GetEntityAsync<ProductDataModel>(
                        "test",
                        "products",
                        "TECH",
                        "PROD1",
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(
                QueryResult.Single(ProductDataModel.New("tech", "prod1", "laptop", "2010"))
            );

        var commandService = new Mock<ICommandService>();
        commandService
            .Setup(
                x =>
                    x.UpdateAsync(
                        "test",
                        "products",
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
                        "test",
                        It.IsAny<CancellationToken>(),
                        It.IsAny<(string, Func<string>)>()
                    )
            )
            .ReturnsAsync(QueueOperation.Failure(QueueOperationError.New(1, "publish error")));

        var response = await Service.ChangeLocation(
            new ChangeLocationRequest
            {
                Category = "tech",
                Id = "prod1",
                LocationCode = "2020"
            },
            validator.Object,
            new UpdateProductSettings("test", "productupdates", "connection string"),
            new RegisterProductSettings("test", "products", "products", "connection string"),
            queueService.Object,
            queryService.Object,
            commandService.Object,
            It.IsAny<CancellationToken>()
        );

        var result = response.Result as ProblemHttpResult;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        result.ContentType.Should().Be(ProblemHeaderType);
        result.ProblemDetails.Title.Should().Be("1");
        result.ProblemDetails.Type.Should().Be("Error");
        result.ProblemDetails.Detail.Should().Be("publish error");
    }

    [Fact(DisplayName = "Updating location successfully")]
    public static async Task ProductUpdateSuccessful()
    {
        var validator = new Mock<IValidator<ChangeLocationRequest>>();
        validator
            .Setup(
                x =>
                    x.ValidateAsync(
                        It.IsAny<ChangeLocationRequest>(),
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(new ValidationResult());

        var queryService = new Mock<IQueryService>();
        queryService
            .Setup(
                x =>
                    x.GetEntityAsync<ProductDataModel>(
                        "test",
                        "products",
                        "TECH",
                        "PROD1",
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(
                QueryResult.Single(ProductDataModel.New("tech", "prod1", "laptop", "2010"))
            );

        var commandService = new Mock<ICommandService>();
        commandService
            .Setup(
                x =>
                    x.UpdateAsync(
                        "test",
                        "products",
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
                        "test",
                        It.IsAny<CancellationToken>(),
                        It.IsAny<(string, Func<string>)>()
                    )
            )
            .ReturnsAsync(QueueOperation.Success());

        var response = await Service.ChangeLocation(
            new ChangeLocationRequest
            {
                Category = "tech",
                Id = "prod1",
                LocationCode = "2020"
            },
            validator.Object,
            new UpdateProductSettings("test", "productupdates", "connection string"),
            new RegisterProductSettings("test", "products", "products", "connection string"),
            queueService.Object,
            queryService.Object,
            commandService.Object,
            It.IsAny<CancellationToken>()
        );

        var result = response.Result as NoContent;
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
}
