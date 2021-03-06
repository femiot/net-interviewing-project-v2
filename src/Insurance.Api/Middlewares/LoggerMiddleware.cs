using Insurance.Shared.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Insurance.Api.Middlewares
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<LoggerMiddleware> logger, IOptions<AppConfiguration> appConfigurationOptions)
        {
            var header = context.Request.Headers[appConfigurationOptions.Value.CorrelationKey];
            if (header.Count > 0)
            {
                using (logger.BeginScope("{@CorrelationId}", header[0]))
                {
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
