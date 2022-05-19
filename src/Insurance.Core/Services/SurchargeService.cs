using Insurance.Core.Interfaces;
using Insurance.Shared.Entities;
using Microsoft.AspNetCore.Http;

namespace Insurance.Core.Services
{
    public class SurchargeService : ISurchargeService
    {
        private readonly IInsuranceUnitOfWork _insuranceUnitOfWork;
        public SurchargeService(IInsuranceUnitOfWork insuranceUnitOfWork)
        {
            _insuranceUnitOfWork = insuranceUnitOfWork;
        }

        public async Task<bool> CaptureRates(IFormFile formFile)
        {
            return true;
        }

        public async Task<float> GetSurchargeByProductTypeIdAsync(int productTypeId)
        {
            var surcharge = await _insuranceUnitOfWork.Repository<ProductTypeSurchargeCost>().FirstOrDefaultAsync(x => x.ProductTypeId == productTypeId);
            return surcharge == null ? 0 : surcharge.Rate;
        }
    }
}
