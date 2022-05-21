using Insurance.Api.Middlewares;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Tests.Middlewares
{
    public class ExceptionHandlerMiddlerwareTests
    {
        Mock<ILogger<ExceptionHandlerMiddleware>> _loggerMock;
        Mock<IWebHostEnvironment> _environmentMock;
        public ExceptionHandlerMiddlerwareTests()
        {
            _loggerMock = new Mock<ILogger<ExceptionHandlerMiddleware>>();
            _environmentMock = new Mock<IWebHostEnvironment>();
        }


        [Fact]
        public async Task ExceptionMiddleware_Given_A_Valid_Request_Processes_Successfully()
        {
            var expectedValue = "Ok";

            var middleware = new ExceptionHandlerMiddleware(
                next: (innerHttpContext) =>
                {
                    innerHttpContext.Response.WriteAsync(expectedValue);
                    return Task.CompletedTask;
                }
            );

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context, _loggerMock.Object, _environmentMock.Object);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = new StreamReader(context.Response.Body).ReadToEnd();

            Assert.Equal(expectedValue, body);
        }


        [Theory]
        [InlineData("Production", "An error occurred please try again")]
        [InlineData("Development", "Something broke within the framework")]
        public async Task ExceptionMiddleware_Given_An_Valid_Request_Handles_Exceptions_Successfully(string environment, string message)
        {
            var middleware = new ExceptionHandlerMiddleware(
                next: (innerHttpContext) =>
                {
                    throw new Exception("Something broke within the framework");
                }
            );

            _environmentMock.Setup(x => x.EnvironmentName).Returns(environment);

            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();

            await middleware.InvokeAsync(context, _loggerMock.Object, _environmentMock.Object);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var body = new StreamReader(context.Response.Body).ReadToEnd();

            Assert.True(body?.Contains(message));
        }
    }
}
