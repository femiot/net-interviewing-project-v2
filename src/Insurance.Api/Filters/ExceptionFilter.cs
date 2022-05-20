using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Insurance.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionFilter(
            ILogger<ExceptionFilter> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "System Error");

            var jsonResult = new JsonResult(new { Message = context.Exception.ToString() });

            if (!_env.IsDevelopment())
                jsonResult = new JsonResult(new { Message = "An error occurred. Please contact administrator" });

            jsonResult.ExecuteResult(context);

            context.ExceptionHandled = true;
            context.Result = jsonResult;
        }
    }
}
