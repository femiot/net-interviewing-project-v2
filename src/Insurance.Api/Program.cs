using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Insurance.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Insurance API is crashing!");
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var builder = Host.CreateDefaultBuilder(args);

            return builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                Log.Logger = new LoggerConfiguration().ReadFrom
                .Configuration(configBuilder.Build())
                .Enrich.FromLogContext().CreateLogger();
            }).ConfigureLogging(logging =>
            {
                logging.AddSerilog();

            }).ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
        }
    }
}
