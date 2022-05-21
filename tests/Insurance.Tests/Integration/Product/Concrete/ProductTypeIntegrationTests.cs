using Insurance.Integration.Product.Concrete;
using Insurance.Shared.AppSettings;
using Insurance.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Tests.Integration.Product.Concrete
{
    public class ProductTypeIntegrationTests
    {
        private Mock<FakeHttpMessageHandler> _fakeHttpMessageHandler;
        private Mock<ILogger<ProductTypeIntegration>> _loggerMock;
        private readonly Mock<IMemoryCache> _memoryCacheMock;
        private Mock<IHttpClientFactory> _httpClientFactoryMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly IOptions<AppConfiguration> _appConfigurationOptions;
        private readonly ProductTypeIntegration _integrationToTest;

        public ProductTypeIntegrationTests()
        {
            _fakeHttpMessageHandler = new Mock<FakeHttpMessageHandler> { CallBase = true };
            _loggerMock = new Mock<ILogger<ProductTypeIntegration>>();
            _memoryCacheMock = new Mock<IMemoryCache>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var httpClient = new HttpClient(_fakeHttpMessageHandler.Object);
            httpClient.BaseAddress = new Uri("https://localhost:2000");
            _httpClientFactoryMock.Setup(cf => cf.CreateClient(It.IsAny<string>())).Returns(httpClient).Verifiable();

            _appConfigurationOptions = Options.Create(new AppConfiguration { CorrelationKey = "test-correlation-id" });

            var context = new DefaultHttpContext();
            context.Request.Headers["test-correlation-id"] = "121323";
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(context);

            _integrationToTest = new ProductTypeIntegration(_loggerMock.Object,
                _memoryCacheMock.Object, _httpClientFactoryMock.Object,
                _appConfigurationOptions, _httpContextAccessorMock.Object);
        }

        [Fact]
        public async Task GetAllProductTypesAsync_Given_Bad_Request_Should_Throw_Exception()
        {
            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadGateway,
                Content = new StringContent("{\"message\": \"error\"]}")
            });

            await Assert.ThrowsAsync<Exception>(() => _integrationToTest.GetAllProductTypesAsync());

        }

        [Fact]
        public async Task GetProductTypeByIdAsync_Given_Bad_Request_Should_Throw_Exception()
        {
            _fakeHttpMessageHandler.Setup(f => f.Send(It.IsAny<HttpRequestMessage>())).Returns(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadGateway,
                Content = new StringContent("{\"message\": \"error\"]}")
            });

            await Assert.ThrowsAsync<Exception>(() => _integrationToTest.GetProductTypeByIdAsync(1));

        }

        [Fact]
        public async Task GetAllProductTypesAsync_Cache_Should_Return_List()
        {
            object expectedValue;

            _memoryCacheMock
           .Setup(x => x.TryGetValue(It.IsAny<string>(), out expectedValue))
           .Returns(true);

            var result = await _integrationToTest.GetAllProductTypesAsync();

            _loggerMock.VerifyLogCall(LogLevel.Information, "Retrieved all product types from cache. Method GetAllProductTypesAsync", 1);
        }
    }
}
