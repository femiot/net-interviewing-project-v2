using Dawn;
using Insurance.Core.Interfaces;
using Insurance.Shared.Constants;
using Insurance.Shared.Payload.Requests;
using Insurance.Shared.Payload.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Insurance.Api.Controllers
{

    [Route("api/[Controller]")]
    public class SurchargeController : Controller
    {
        private readonly ISurchargeService _surchargeService;
        private readonly IHttpContextAccessor _contextAccessor;
        public SurchargeController(ISurchargeService surchargeService, IHttpContextAccessor contextAccessor)
        {
            _surchargeService = Guard.Argument(surchargeService, nameof(surchargeService)).NotNull().Value;
            _contextAccessor = Guard.Argument(contextAccessor, nameof(contextAccessor)).NotNull().Value;
        }

        [HttpPost("UploadSurchargeRates")]
        public async Task<SurchargeUploadResponse> UploadSurchargeRates([FromForm] SurchargeUploadRequest payload)
        {
            _contextAccessor.HttpContext.Request.Headers.Add(HeaderConstants.CurrentUserId, payload.UserId.ToString());
            var uploaded = await _surchargeService.CaptureRatesAsync(payload.BuildSurchageRateFromFile());
            return new SurchargeUploadResponse { SuccessfullyUploaded = uploaded };
        }
    }
}