using Insurance.Api.Middlewares;
using Insurance.Shared.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Tests.Middlewares
{
    public class LoggerMiddlewareTests
    {
        Mock<ILogger<LoggerMiddleware>> _loggerMock;
        IOptions<AppConfiguration> _appConfigurationOptions;
        public LoggerMiddlewareTests()
        {
            _loggerMock = new Mock<ILogger<LoggerMiddleware>>();
            _appConfigurationOptions = Options.Create(new AppConfiguration { CorrelationKey = "test-correlation-id" });
        }


        [Fact]
        public async Task ExceptionMiddleware_Given_A_Valid_Request_Processes_Successfully()
        {
            var expectedValue = "Ok";


            var middleware = new LoggerMiddleware(
                next: (innerHttpContext) =>
                {
                    innerHttpContext.Response.WriteAsync(expectedValue);
                    return Task.CompletedTask;
                }
            );

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context, _loggerMock.Object, _appConfigurationOptions);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = new StreamReader(context.Response.Body).ReadToEnd();

            Assert.Equal(expectedValue, body);
        }
    }
}
