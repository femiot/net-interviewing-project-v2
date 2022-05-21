using Insurance.Infrastructure.EF;
using Insurance.Shared.Payload.Responses;
using Insurance.Tests.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace Insurance.Tests.Controllers
{
    [Collection("Tests collection")]
    public class SurchargeIntegrationTests
    {
        public SurchargeIntegrationTests(TestFixture<TestStartup> fixture)
        {
            Client = fixture.Client;
            Services = fixture.Services;
        }

        public HttpClient Client { get; }
        public IServiceProvider Services { get; }

        [Theory]
        [InlineData("POST")]
        public async Task UploadSurchargeRates_Given_A_Valid_File_And_UserId_Should_Upload_Successfully(string method)
        {
            var userId = Guid.NewGuid().ToString();
            var content = "ProductTypeId| SurchargeRate" + Environment.NewLine +
                "32 | 2300,123" + Environment.NewLine +
                "33 | 500,45" + Environment.NewLine +
                "21 | 1000,34";
            var fileName = "test.csv";
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            IFormFile file = new FormFile(stream, 0, bytes.Length, "data", fileName);

            var fileContent = new StreamContent(file.OpenReadStream())
            {
                Headers =
                {
                    ContentLength = file.Length,
                    ContentType = new MediaTypeHeaderValue("text/csv")
                }
            };

            var formDataContent = new MultipartFormDataContent();
            formDataContent.Add(fileContent, "SurchargeFile", file.FileName);
            formDataContent.Add(new StringContent(userId), "UserId");

            var request = new HttpRequestMessage(new HttpMethod(method), "/api/surcharge/UploadSurchargeRates");
            request.Content = formDataContent;

            //Act
            var response = await Client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<SurchargeUploadResponse>(responseContent);
            var database = Services.GetService<InsuranceContext>();
            var surcharges = database.ProductTypeSurchargeCosts.Where(x => (x.ProductTypeId == 32
            || x.ProductTypeId == 33 || x.ProductTypeId == 21) && x.CreatedByUserId == userId);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.True(result.SuccessfullyUploaded);
            Assert.True(surcharges.Count() == 3);

        }

        [Theory]
        [InlineData("POST")]
        public async Task UploadSurchargeRates_Given_An_Invalid_File_And_UserId_Should_Return_Validation_Message(string method)
        {
            var userId = Guid.Empty.ToString();
            var content = "ProductTypeId| SurchargeRate" + Environment.NewLine +
                "32 | 2300,123" + Environment.NewLine +
                "33 | 500,45" + Environment.NewLine +
                "21 | 1000,34";
            var fileName = "test.txt";
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            IFormFile file = new FormFile(stream, 0, bytes.Length, "data", fileName);

            var fileContent = new StreamContent(file.OpenReadStream())
            {
                Headers =
                {
                    ContentLength = file.Length,
                    ContentType = new MediaTypeHeaderValue("text/plain")
                }
            };

            var formDataContent = new MultipartFormDataContent();
            formDataContent.Add(fileContent, "SurchargeFile", file.FileName);
            formDataContent.Add(new StringContent(userId), "UserId");

            var request = new HttpRequestMessage(new HttpMethod(method), "/api/surcharge/UploadSurchargeRates");
            request.Content = formDataContent;

            //Act
            var response = await Client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.NotNull(responseContent);
            Assert.True(responseContent?.Contains("UserId"));
            Assert.True(responseContent?.Contains("Please provide User Id"));
            Assert.True(responseContent?.Contains("SurchargeFile"));
            Assert.True(responseContent?.Contains("Please upload a valid csv file (1 GB size max)"));
        }

    }
}
