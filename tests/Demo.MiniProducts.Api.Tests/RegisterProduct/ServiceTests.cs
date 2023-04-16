using Azure.Storage.Table.Wrapper.Queries;
using Demo.MiniProducts.Api.DataAccess;
using Demo.MiniProducts.Api.Features;
using Demo.MiniProducts.Api.Features.FindById;
using Demo.MiniProducts.Api.Features.RegisterProduct;
using FluentAssertions;
using LanguageExt.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Moq;
using Service = Demo.MiniProducts.Api.Features.FindById.Service;

namespace Demo.MiniProducts.Api.Tests.RegisterProduct;

public class ServiceTests
{
    private const string StorageCategory = "test";
    private const string StorageTable = "products";
    private const string StorageQueue = "products";
    private const string ProblemHeaderType = "application/problem+json";

    private static RegisterProductSettings GetSettings() =>
        new(StorageCategory, StorageQueue, StorageTable, "connection string");

    [Fact]
    public async Task ErrorWhenQuerying()
    {
        var logger = new Mock<ILogger>();

        var queryService = new Mock<IQueryService>();
        queryService
            .Setup(
                x =>
                    x.GetEntityAsync<ProductDataModel>(
                        StorageCategory,
                        StorageTable,
                        "tech",
                        "prod1",
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(QueryResult.Fail(Error.New(1, "error")));
        var op = await Service.GetProductDetailsById(
            "tech",
            "prod1",
            GetSettings(),
            queryService.Object,
            logger.Object
        );
        var problem = op.Result as ProblemHttpResult;
        problem.Should().NotBeNull();
        problem!.ContentType.Should().Be(ProblemHeaderType);
        problem!.ProblemDetails.Type.Should().Be("Error");
        problem.ProblemDetails.Title.Should().Be(ErrorCodes.FindProductError.ToString());
        problem.ProblemDetails.Detail.Should().Be(ErrorMessages.FindProductError);
        problem!.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        logger.VerifyLog(LogLevel.Error, Times.Once());
    }

    [Fact]
    public async Task ProductExists()
    {
        var logger = new Mock<ILogger>();

        var queryService = new Mock<IQueryService>();
        queryService
            .Setup(
                x =>
                    x.GetEntityAsync<ProductDataModel>(
                        StorageCategory,
                        StorageTable,
                        "TECH",
                        "PROD1",
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(
                () => QueryResult.Single(ProductDataModel.New("tech", "prod1", "laptop", "2010"))
            );
        var op = await Service.GetProductDetailsById(
            "TECH",
            "PROD1",
            GetSettings(),
            queryService.Object,
            logger.Object
        );
        var response = op.Result as Ok<ProductResponse>;
        response.Should().NotBeNull();
        response!.Value.Should().NotBeNull();
        response.Value!.Data.Category.Should().Be("tech");
        response.Value.Data.Id.Should().Be("prod1");
        response.Value.Data.Name.Should().Be("laptop");
        response.Value.Data.Location.Should().Be("2010");
        logger.VerifyLog(LogLevel.Information, Times.Once());
    }

    [Fact]
    public async Task ProductDoesNotExists()
    {
        var logger = new Mock<ILogger>();

        var queryService = new Mock<IQueryService>();
        queryService
            .Setup(
                x =>
                    x.GetEntityAsync<ProductDataModel>(
                        StorageCategory,
                        StorageTable,
                        "TECH",
                        "PROD1",
                        It.IsAny<CancellationToken>()
                    )
            )
            .ReturnsAsync(() => QueryResult.Empty());
        var op = await Service.GetProductDetailsById(
            "TECH",
            "PROD1",
            GetSettings(),
            queryService.Object,
            logger.Object
        );
        var response = op.Result as NotFound;
        response.Should().NotBeNull();
    }
}
