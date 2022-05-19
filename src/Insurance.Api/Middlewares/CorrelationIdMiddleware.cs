using Insurance.Shared.AppSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace Insurance.Api.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppConfiguration _appConfiguration;
        private readonly ILogger _logger;

        public CorrelationIdMiddleware(RequestDelegate next, IOptions<AppConfiguration> appConfigurationOptions, ILoggerFactory loggerFactory)
        {
            if (appConfigurationOptions == null)
                throw new ArgumentNullException(nameof(appConfigurationOptions));

            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = loggerFactory.CreateLogger<CorrelationIdMiddleware>();

            _appConfiguration = appConfigurationOptions.Value;
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue(_appConfiguration.CorrelationKey, out StringValues correlationId))
            {
                context.TraceIdentifier = correlationId;
                _logger.LogInformation($"CorrelationId from Request Header:{ correlationId}");
            }
            else
            {
                correlationId = Guid.NewGuid().ToString();
                context.TraceIdentifier = correlationId;
                context.Request.Headers.Add(_appConfiguration.CorrelationKey, correlationId);
                _logger.LogInformation($"Generated CorrelationId:{ correlationId}");
            }

            context.Response.OnStarting(() =>
            {
                context.Response.Headers.Add(_appConfiguration.CorrelationKey, new[] { context.TraceIdentifier });
                return Task.CompletedTask;
            });

            return _next(context);
        }
    }
}
