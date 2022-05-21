using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace Insurance.Tests.Helpers
{
    public static class AssertHelper
    {
        public static void VerifyLogCall<T>(this Mock<ILogger<T>> loggerMock, LogLevel level, string message, int times)
        {
            loggerMock.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == level),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == message && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),Times.Exactly(times));
        }
    }
}
