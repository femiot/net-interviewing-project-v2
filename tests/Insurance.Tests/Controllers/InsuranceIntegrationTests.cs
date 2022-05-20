using Insurance.Shared.Payload.Requests;
using Insurance.Shared.Payload.Responses;
using Insurance.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Insurance.Tests.Controllers
{
    public class InsuranceIntegrationTests : IClassFixture<TestFixture<TestStartup>>
    {
        public InsuranceIntegrationTests(TestFixture<TestStartup> fixture)
        {
            Client = fixture.Client;
        }

        public HttpClient Client { get; }

        [Theory]
        [InlineData("POST")]
        public async Task CalculateInsurance_GivenSalesPriceBetween500And2000Euros_ShouldAddThousandEurosToInsuranceCost(string method)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/insurance/product");
            request.Content = new StringContent(JsonSerializer.Serialize(new InsuranceRequest 
            { 
                ProductId = 1,
            }), Encoding.UTF8, "application/json");

            var expectedInsuranceValue = 1000;

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<InsuranceResponse>(content);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.InsuranceValue
            );
        }
    }
}
