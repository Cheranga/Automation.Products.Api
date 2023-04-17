using Microsoft.Extensions.Logging;
using Moq;

namespace Demo.MiniProducts.Api.Tests;

public static class MockHelpers
{
    public static void VerifyLog(this Mock<ILogger> mockedLogger, LogLevel level, Times times)
    {
        mockedLogger.Verify(
            x =>
                x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()
                ),
            times
        );
    }
}