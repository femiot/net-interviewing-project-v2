using Insurance.Shared.DTOs;

namespace Insurance.Core.Interfaces
{
    public interface ISurchargeService
    {
        Task<bool> CaptureRatesAsync(IEnumerable<SurchargeRateDto> surchargeRateDtos);
        Task<float> GetSurchargeByProductTypeIdAsync(int productTypeId);
        Task<float> GetSurchargeByProductTypeIdsAsync(int[] productTypeIds);
    }
}
