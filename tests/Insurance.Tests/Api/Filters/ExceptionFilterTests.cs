using Insurance.Api.Filters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Routing;

namespace Insurance.Tests.Api.Filters
{
    public class ExceptionFilterTests
    {

        private readonly Mock<ILogger<ExceptionFilter>> _loggerMock;
        private readonly Mock<IWebHostEnvironment> _envMock;

        private readonly ExceptionFilter _exceptionFilter;

        public ExceptionFilterTests()
        {
            _loggerMock = new Mock<ILogger<ExceptionFilter>>();
            _envMock = new Mock<IWebHostEnvironment>();

            _exceptionFilter = new ExceptionFilter(_loggerMock.Object, _envMock.Object);
        }

        [Fact]
        public async Task ExceptionFilter_Given_Production_Flag_Should_Return_Friendly_Message()
        {
            var defaultHttpContext = new DefaultHttpContext();
            ActionContext actionContext = new Mock<ActionContext>().Object;
            actionContext.HttpContext = defaultHttpContext;
            actionContext.RouteData = new Mock<RouteData>().Object;
            actionContext.ActionDescriptor = new Mock<ActionDescriptor>().Object;

            var mockException = new Mock<Exception>();

            mockException.Setup(e => e.StackTrace)
              .Returns("Test stacktrace");
            mockException.Setup(e => e.Message)
              .Returns("Test message");
            mockException.Setup(e => e.Source)
              .Returns("Test source");
            var exceptionContext = new ExceptionContext(actionContext, new List<IFilterMetadata>())
            {
                Exception = mockException.Object,
                HttpContext = defaultHttpContext
            };

            _envMock.Setup(x => x.EnvironmentName).Returns("Production");

            _exceptionFilter.OnException(exceptionContext);

            var jsonResult = exceptionContext.Result as JsonResult;
            Assert.NotNull(jsonResult);
            Assert.True(jsonResult?.Value.ToString().Contains("An error occurred. Please contact administrator"));
        }
    }
}
