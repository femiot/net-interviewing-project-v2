using Dawn;
using Insurance.Core.Interfaces;
using Insurance.Shared.AppSettings;
using Insurance.Shared.Constants;
using Insurance.Shared.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Insurance.Integration.Product.Concrete
{
    public class ProductIntegration : IProductIntegration
    {
        private ILogger<ProductIntegration> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly HttpClient _httpClient;
        private readonly string _correlationId;
        public ProductIntegration(ILogger<ProductIntegration> logger,
            IMemoryCache memoryCache,
            IHttpClientFactory httpClientFactory,
            IOptions<AppConfiguration> appConfigurationOptions,
            IHttpContextAccessor contextAccessor)
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            _memoryCache = Guard.Argument(memoryCache, nameof(memoryCache)).NotNull().Value;
            httpClientFactory = Guard.Argument(httpClientFactory, nameof(httpClientFactory)).NotNull().Value;
            contextAccessor = Guard.Argument(contextAccessor, nameof(contextAccessor)).NotNull().Value;

            _httpClient = httpClientFactory.CreateClient(IntegrationConstants.ProductDataApi);
            _correlationId = contextAccessor.HttpContext.Request.Headers[appConfigurationOptions.Value.CorrelationKey].ToString();
            _httpClient.DefaultRequestHeaders.Add(appConfigurationOptions.Value.CorrelationKey, _correlationId);
        }

        public async Task<ProductIntegrationDto?> GetProductByIdAsync(int productId)
        {
            _logger.LogInformation($"Calling product data api for Product with ID {productId}. Method {nameof(GetProductByIdAsync)}");

            var response = await _httpClient.GetAsync(string.Format("/products/{0:G}", productId));

            if (response != null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Payload recieved from product data api {content}. Method {nameof(GetProductByIdAsync)}");

                return JsonSerializer.Deserialize<ProductIntegrationDto>(content);
            }

            var integrationErrorMessage = response != null ? await response.Content.ReadAsStringAsync() : string.Empty;

            var errorMessage = $"Product for ID: {productId} not found in method {nameof(GetProductByIdAsync)}. Integration Error Message {integrationErrorMessage}";
            throw new Exception(errorMessage);
        }

        public async Task<List<ProductIntegrationDto>?> GetAllProductsAsync()
        {          

            if (_memoryCache.TryGetValue(nameof(List<ProductIntegrationDto>), out List<ProductIntegrationDto>? productIntegrationList))
            {
                _logger.LogInformation($"Retrieved all products from cache. Method {nameof(GetAllProductsAsync)}");
                return productIntegrationList;
            }

            _logger.LogInformation($"Calling product data api to get all products. Method {nameof(GetAllProductsAsync)}");

            var response = await _httpClient.GetAsync("/products");

            if (response != null && response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"Payload recieved from product data api {content}. Method {nameof(GetAllProductsAsync)}");

                productIntegrationList = JsonSerializer.Deserialize<List<ProductIntegrationDto>?>(content);
                if (productIntegrationList != null)
                    _memoryCache.Set(nameof(List<ProductIntegrationDto>), productIntegrationList);

                return productIntegrationList;
            }

            var integrationErrorMessage = response != null ? await response.Content.ReadAsStringAsync() : string.Empty;

            var errorMessage = $"Products not found in method {nameof(GetAllProductsAsync)}. Integration Error Message {integrationErrorMessage}";
            throw new Exception(errorMessage);
        }
    }
}
