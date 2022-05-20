using Insurance.Shared.Payload.Requests;
using Insurance.Shared.Payload.Responses;
using Insurance.Tests.Helpers;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text;
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
        public async Task CalculateInsurance_GivenSalesPriceBetween500And2000Euros_ShouldReturnZero_Cannot_Be_Insured_Product(string method)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/insurance/product");
            request.Content = new StringContent(JsonConvert.SerializeObject(new InsuranceRequest
            {
                ProductId = 725435,
            }), Encoding.UTF8, "application/json");

            var expectedInsuranceValue = 0;

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<InsuranceResponse>(content);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.InsuranceValue
            );
        }

        [Theory]
        [InlineData("POST")]
        public async Task CalculateInsurance_GivenSalesPriceBetween500And2000Euros_ShouldAddThousandEurosToInsuranceCost(string method)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/insurance/product");
            request.Content = new StringContent(JsonConvert.SerializeObject(new InsuranceRequest 
            { 
                ProductId = 735246,
            }), Encoding.UTF8, "application/json");

            var expectedInsuranceValue = 1000;

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<InsuranceResponse>(content);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.InsuranceValue
            );
        }

        [Theory]
        [InlineData("POST", 837856)]
        [InlineData("POST", 828519)]
        [InlineData("POST", 780829)]
        public async Task CalculateInsurance_GivenSalesPriceLessThan500_ShouldAdd500EurosToInsuranceCost_On_Specific_Products(string method, int productId)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), "/api/insurance/product");
            request.Content = new StringContent(JsonConvert.SerializeObject(new InsuranceRequest
            {
                ProductId = productId,
            }), Encoding.UTF8, "application/json");

            var expectedInsuranceValue = 500;

            // Act
            var response = await Client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<InsuranceResponse>(content);

            Assert.Equal(
                expected: expectedInsuranceValue,
                actual: result.InsuranceValue
            );
        }
    }
}
