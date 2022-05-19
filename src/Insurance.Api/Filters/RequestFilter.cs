using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

namespace Insurance.Api.Filters
{
    public class RequestFilter : IActionFilter
    {
        ILogger<RequestFilter> _logger;
        public RequestFilter(ILogger<RequestFilter> logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation($"RequestFilter: {context.ActionDescriptor.DisplayName} Execution completed");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation($"RequestFilter: {context.ActionDescriptor.DisplayName} Execution started");
        }
    }
}
