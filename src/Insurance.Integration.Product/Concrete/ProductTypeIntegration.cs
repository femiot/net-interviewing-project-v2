using Dawn;
using Insurance.Core.Interfaces;
using Insurance.Shared.AppSettings;
using Insurance.Shared.Constants;
using Insurance.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Insurance.Integration.Product.Concrete
{
    public class ProductTypeIntegration : IProductTypeIntegration
    {
        private ILogger<ProductTypeIntegration> _logger;
        private readonly HttpClient _httpClient;
        private readonly string _correlationId;
        public ProductTypeIntegration(ILogger<ProductTypeIntegration> logger, IHttpClientFactory httpClientFactory,
            IOptions<AppConfiguration> appConfigurationOptions, IHttpContextAccessor contextAccessor)
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            httpClientFactory = Guard.Argument(httpClientFactory, nameof(httpClientFactory)).NotNull().Value;
            contextAccessor = Guard.Argument(contextAccessor, nameof(contextAccessor)).NotNull().Value;

            _httpClient = httpClientFactory.CreateClient(IntegrationConstants.ProductDataApi);
            _correlationId = contextAccessor.HttpContext.Request
                .Headers[appConfigurationOptions.Value.CorrelationKey].ToString();
            _httpClient.DefaultRequestHeaders.Add(appConfigurationOptions.Value.CorrelationKey, _correlationId);
            _httpClient.BaseAddress = new Uri(appConfigurationOptions.Value.ProductIntegrationServiceUrl);
        }

        public async Task<ProductTypeIntegrationDto?> GetProductTypeByIdAsync(int productTypeId)
        {
            _logger.LogInformation($"Calling product data api for Product Type with ID {productTypeId}. Method {nameof(GetProductTypeByIdAsync)}");

            var response = await _httpClient.GetAsync(string.Format("/product_types/{0:G}", productTypeId));

            if (response != null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Payload recieved from product data api {content}. Method {nameof(GetProductTypeByIdAsync)}");

                return JsonConvert.DeserializeObject<ProductTypeIntegrationDto>(content);
            }

            var integrationErrorMessage = response != null ? await response.Content.ReadAsStringAsync() : string.Empty;

            var errorMessage = $"Product Type for ID: {productTypeId} not found in method {nameof(GetProductTypeByIdAsync)}. Integration Error Message {integrationErrorMessage}";
            _logger.LogError(errorMessage);
            throw new Exception(errorMessage);
        }
    }
}
