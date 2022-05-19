using Dawn;
using Insurance.Core.Interfaces;
using Insurance.Shared.DTOs;
using Insurance.Shared.Entities;
using Microsoft.Extensions.Logging;

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
                var extraCost = await GetExtraCostInsuranceValueAsync(productType, productId);
                var surchargeCost = await _surchargeService.GetSurchargeByProductTypeIdAsync(product.ProductTypeId);
                insuranceCost = ruleBasedCost + extraCost + surchargeCost;

                _logger.LogInformation($"Total Insurance {insuranceCost} calculated for Product with ID {productId}");
            }

            return insuranceCost;
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

        private async Task<float> GetExtraCostInsuranceValueAsync(ProductTypeIntegrationDto productTypeIntegrationDto, int productId)
        {
            _logger.LogInformation($"Calculating extra cost insurance for Product with ID {productId}");

            var insuranceExtraCost = await _insuranceUnitOfWork.Repository<InsuranceExtraCost>().FirstOrDefaultAsync(x =>
            productTypeIntegrationDto.Name == x.ProductName && x.ApplyCostRangeRule);

            var value = insuranceExtraCost == null ? 0 : insuranceExtraCost.ExtraCost;

            _logger.LogInformation($"Product Insurance Value {value} for Product with ID {productId}");

            return value;
        }
    }
}
