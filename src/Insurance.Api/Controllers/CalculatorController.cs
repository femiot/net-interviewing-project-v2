using Dawn;
using Insurance.Core.Interfaces;
using Insurance.Shared.Payload.Requests;
using Insurance.Shared.Payload.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Insurance.Api.Controllers
{

    [Route("api/insurance")]
    public class CalculatorController : Controller
    {
        private readonly ICalculatorService _calculatorService;
        public CalculatorController(ICalculatorService calculatorService)
        {
            _calculatorService = Guard.Argument(calculatorService, nameof(calculatorService)).NotNull().Value;
        }

        [HttpPost("product")]
        public async Task<InsuranceResponse> CalculateInsurance([FromBody] InsuranceRequest payload)
        {
            var value = await _calculatorService.CalculateProductInsuranceValueAsync(productId: payload.ProductId);
            return new InsuranceResponse { ProductId = payload.ProductId, InsuranceValue = value };
        }

        [HttpPost("products")]
        public async Task<InsuranceProductsResponse> CalculateInsurance([FromBody] List<InsuranceRequest> payload)
        {
            var value = await _calculatorService.CalculateProductsInsuranceValueAsync(productIds: payload.Select(x => x.ProductId).ToArray());
            return new InsuranceProductsResponse { InsuranceTotalValue = value };
        }
    }
}