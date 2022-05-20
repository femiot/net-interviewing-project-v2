using Dawn;
using Insurance.Core.Interfaces;
using Insurance.Shared.Constants;
using Insurance.Shared.DTOs;
using Insurance.Shared.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Insurance.Core.Services
{
    public class SurchargeService : ISurchargeService
    {
        private readonly ILogger<SurchargeService> _logger;
        private readonly IInsuranceUnitOfWork _insuranceUnitOfWork;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IGenericRepository<ProductTypeSurchargeCost> _productTypeSurchargeCostRepository;
        public SurchargeService(ILogger<SurchargeService> logger, IInsuranceUnitOfWork insuranceUnitOfWork, IHttpContextAccessor contextAccessor)
        {
            _logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            _insuranceUnitOfWork = Guard.Argument(insuranceUnitOfWork, nameof(insuranceUnitOfWork)).NotNull().Value;
            _productTypeSurchargeCostRepository = _insuranceUnitOfWork.Repository<ProductTypeSurchargeCost>();
            _contextAccessor = Guard.Argument(contextAccessor, nameof(contextAccessor)).NotNull().Value;
        }

        public async Task<bool> CaptureRatesAsync(IEnumerable<SurchargeRateDto> surchargeRateDtos)
        {
            Guard.Argument(surchargeRateDtos, nameof(surchargeRateDtos)).NotNull();

            _contextAccessor.HttpContext.Request.Headers.TryGetValue(HeaderConstants.CurrentUserId, out var userId);

            foreach (SurchargeRateDto surchargeRateDto in surchargeRateDtos)
            {
                var existingRate = await _productTypeSurchargeCostRepository.FirstOrDefaultAsync(x => x.ProductTypeId == surchargeRateDto.ProductTypeId);
                if (existingRate != null)
                {
                    _logger.LogInformation($"User with ID {userId} Updating surcharge rate for Product Type Id {surchargeRateDto.ProductTypeId} - Rate {surchargeRateDto.SurchargeRate}");
                    existingRate.Rate = surchargeRateDto.SurchargeRate;
                    _productTypeSurchargeCostRepository.Update(existingRate);
                }
                else
                {
                    _logger.LogInformation($"User with ID {userId} Adding surcharge rate for Product Type Id {surchargeRateDto.ProductTypeId} - Rate {surchargeRateDto.SurchargeRate}");
                    var newSurchargeRate = new ProductTypeSurchargeCost
                    {
                        Rate = surchargeRateDto.SurchargeRate,
                        ProductTypeId = surchargeRateDto.ProductTypeId
                    };
                    await _productTypeSurchargeCostRepository.InsertAsync(newSurchargeRate);
                }
            }

            await _insuranceUnitOfWork.SaveAsync();

            return true;
        }

        public async Task<float> GetSurchargeByProductTypeIdAsync(int productTypeId)
        {
            var surcharge = await _insuranceUnitOfWork.Repository<ProductTypeSurchargeCost>().FirstOrDefaultAsync(x => x.ProductTypeId == productTypeId);
            var value = surcharge == null ? 0 : surcharge.Rate;
            _logger.LogInformation($"Retrieved surcharge rate for Product Type Id {productTypeId} - Rate {value}");
            return value;
        }

        public async Task<float> GetSurchargeByProductTypeIdsAsync(int[] productTypeIds)
        {
            var surcharges = await _insuranceUnitOfWork.Repository<ProductTypeSurchargeCost>().FindAsync(x => productTypeIds.Contains(x.ProductTypeId));
            var value = surcharges == null ? 0 : surcharges.Sum(x => x.Rate);
            _logger.LogInformation($"Retrieved surcharge rates for Product Type Ids {JsonSerializer.Serialize(productTypeIds)} - Sum Total Rate {value}");
            return value;
        }
    }
}
