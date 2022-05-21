using Dawn;
using Insurance.Core.Interfaces;
using Insurance.Shared.DTOs;
using Insurance.Shared.Entities;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Insurance.Core.Services
{
    public class CalculatorService : ICalculatorService
    {
        private readonly ILogger<CalculatorService> _logger;
        private readonly IProductIntegration _productIntegration;
        private readonly IProductTypeIntegration _productTypeIntegration;
        private readonly ISurchargeService _surchargeService;
        private IInsuranceUnitOfWork _insuranceUnitOfWork;

        public CalculatorService(ILogger<CalculatorService> logger,
            IProductIntegration productIntegration,
            IProductTypeIntegration productTypeIntegration,
            ISurchargeService surchargeService,
            IInsuranceUnitOfWork insuranceUnitOfWork)
        {
            _logger = Guard.Argument(logger, nameof(productIntegration)).NotNull().Value;
            _productIntegration = Guard.Argument(productIntegration, nameof(productIntegration)).NotNull().Value;
            _productTypeIntegration = Guard.Argument(productTypeIntegration, nameof(productTypeIntegration)).NotNull().Value;
            _surchargeService = Guard.Argument(surchargeService, nameof(surchargeService)).NotNull().Value;
            _insuranceUnitOfWork = Guard.Argument(insuranceUnitOfWork, nameof(insuranceUnitOfWork)).NotNull().Value;
        }

        public async Task<float> CalculateProductInsuranceValueAsync(int productId)
        {
            _logger.LogInformation($"Calculating Product Insurance Value for Product with ID {productId}");

            float insuranceCost = 0;
            var product = await _productIntegration.GetProductByIdAsync(productId);

            if (product == null)
                throw new Exception($"Could not fetch Product by ID {productId}");

            var productType = await _productTypeIntegration.GetProductTypeByIdAsync(product.ProductTypeId);

            if (productType == null)
                throw new Exception($"Could not fetch Product Type by ID {product.ProductTypeId}");

            if (productType.CanBeInsured)
            {
                var ruleBasedCost = await GetRuleBasedInsuranceValueAsync(product);
                var extraCost = await GetExtraCostInsuranceValueAsync(productType, productId, ruleBasedCost);
                var surchargeCost = await _surchargeService.GetSurchargeByProductTypeIdAsync(product.ProductTypeId);
                insuranceCost = ruleBasedCost + extraCost + surchargeCost;

                _logger.LogInformation($"Total Insurance {insuranceCost} calculated for Product with ID {productId}");
            }

            return insuranceCost;
        }

        public async Task<float> CalculateProductsInsuranceValueAsync(int[] productIds)
        {
            _logger.LogInformation($"Calculating Product Insurance Values for Products with IDs {JsonSerializer.Serialize(productIds)}");

            float insuranceCost = 0;
            var products = await _productIntegration.GetAllProductsAsync();

            if (products == null || products.Count == 0)
                throw new Exception($"Could not fetch all Products");

            var productTypes = await _productTypeIntegration.GetAllProductTypesAsync();

            if (productTypes == null)
                throw new Exception($"Could not fetch Product Types");

            return await ProcessProductsInsuranceValueAsync(productIds, insuranceCost, products, productTypes);
        }

        private async Task<float> GetRuleBasedInsuranceValueAsync(ProductIntegrationDto productIntegrationDto)
        {
            _logger.LogInformation($"Calculating Rule based insurance cost (product sales: {productIntegrationDto.SalesPrice}) for Product Type with ID {productIntegrationDto.ProductTypeId}");

            var costRangeRule = await _insuranceUnitOfWork.Repository<CostRangeRule>().FirstOrDefaultAsync(x =>
            (productIntegrationDto.SalesPrice >= x.Min && x.IgnoreMax)
            || (productIntegrationDto.SalesPrice >= x.Min && productIntegrationDto.SalesPrice < x.Max));

            var value = costRangeRule == null ? 0 : costRangeRule.Value;

            _logger.LogInformation($"Product Insurance Value {value} for Product with ID {productIntegrationDto.ProductTypeId}");

            return value;
        }

        public async Task<float> GetExtraCostInsuranceValueAsync(ProductTypeIntegrationDto productTypeIntegrationDto, 
            int productId, float ruleBasedCost)
        {
            _logger.LogInformation($"Calculating extra cost insurance for Product with ID {productId}");

            var insuranceExtraCost = await _insuranceUnitOfWork.Repository<InsuranceExtraCost>().FirstOrDefaultAsync(x =>
            productTypeIntegrationDto.Name == x.ProductName);

            if (insuranceExtraCost != null && insuranceExtraCost.ApplyCostRangeRule && ruleBasedCost == 0)
                return 0;

            var value = insuranceExtraCost == null ? 0 : insuranceExtraCost.ExtraCost;

            _logger.LogInformation($"Product Insurance Value {value} for Product with ID {productId}");

            return value;
        }

        private async Task<float> ProcessProductsInsuranceValueAsync(
            int[] productIds, float insuranceCost,
            List<ProductIntegrationDto> products,
            List<ProductTypeIntegrationDto> productTypes)
        {
            foreach (var productId in productIds)
            {
                var product = products.FirstOrDefault(x => x.Id == productId);
                var productType = productTypes.FirstOrDefault(x => product != null && x.Id == product.ProductTypeId);
                if (product != null && productType != null && productType.CanBeInsured)
                {
                    var ruleBasedCost = await GetRuleBasedInsuranceValueAsync(product);
                    var extraCost = await GetExtraCostInsuranceValueAsync(productType, productId, ruleBasedCost);
                    insuranceCost += ruleBasedCost + extraCost;

                    _logger.LogInformation($"Insurance {ruleBasedCost + extraCost} calculated for Product with ID {productId} excluding surcharges");
                }
            }

            var uniqueProductTypeIds = products.Where(x => productIds.Contains(x.Id)).Select(x => x.ProductTypeId).Distinct().ToArray();
            var canBeInsuredProductTypeIds = productTypes.Where(x => uniqueProductTypeIds.Contains(x.Id) && x.CanBeInsured).Select(x => x.Id).Distinct().ToArray();

            if (canBeInsuredProductTypeIds.Any())
                insuranceCost += await _surchargeService.GetSurchargeByProductTypeIdsAsync(canBeInsuredProductTypeIds);

            return insuranceCost;
        }
    }
}
