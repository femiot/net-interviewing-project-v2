using Microsoft.AspNetCore.Http;

namespace Insurance.Core.Interfaces
{
    public interface ISurchargeService
    {
        Task<bool> CaptureRates(IFormFile formFile);
        Task<float> GetSurchargeByProductTypeIdAsync(int productTypeId);
    }
}
