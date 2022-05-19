using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Insurance.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(
            ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "System Error");
            var jsonResult = new JsonResult("An error occurred. Please contact administrator");
            jsonResult.ExecuteResult(context);
            context.ExceptionHandled = true;
            context.Result = jsonResult;
        }
    }
}
