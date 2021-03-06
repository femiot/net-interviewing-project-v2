using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Insurance.Api.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<ExceptionHandlerMiddleware> logger, IWebHostEnvironment env)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error occurred and handled in {nameof(ExceptionHandlerMiddleware)}");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                if (env.IsDevelopment())
                    await context.Response.WriteAsync(ex.ToString());
                else
                    await context.Response.WriteAsync("An error occurred please try again");
            }
        }
    }
}
