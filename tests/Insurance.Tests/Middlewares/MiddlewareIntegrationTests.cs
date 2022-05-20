using Insurance.Tests.Helpers;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Tests.Middlewares
{
    public class MiddlewareIntegrationTests : IClassFixture<TestFixture<TestStartup>>
    {
        public MiddlewareIntegrationTests(TestFixture<TestStartup> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        [Theory]
        [InlineData("GET")]
        public async Task Given_A_Health_Endpoint_Should_Return_A_Valid_Correlation_Id_From_Middleware(string method)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/alive");

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();


            Assert.Equal("Healthy", content);
            Assert.True(response.Headers.TryGetValues("Coolblue-Correlation-ID", out var values));
            Assert.True(Guid.TryParse(values.First(), out var guidValue));
            Assert.NotEqual(Guid.Empty, guidValue);
        }

        [Theory]
        [InlineData("GET")]
        public async Task Given_A_Correlation_Id_Should_Return_The_Same_Correlation_Id_From_Middleware(string method)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/alive");
            var correlationId = Guid.NewGuid();
            request.Headers.Add("Coolblue-Correlation-ID", correlationId.ToString());

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();


            Assert.Equal("Healthy", content);
            Assert.True(response.Headers.TryGetValues("Coolblue-Correlation-ID", out var values));
            Assert.True(Guid.TryParse(values.First(), out var guidValue));
            Assert.Equal(correlationId, guidValue);
        }
    }
}
