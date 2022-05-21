using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;

namespace Insurance.Tests.Helpers
{
    public class TestFixture<TStartup> : IDisposable where TStartup : class
    {
        private readonly IHost _host;
        public TestFixture()
        {
            _host = new HostBuilder()
                .ConfigureAppConfiguration((context, builder) =>
                {
                    builder.AddJsonFile("appsettings.json");
                })
                  .ConfigureWebHostDefaults(
                       b => b
                       .UseEnvironment("Development")
                       .UseUrls("http://localhost:5000")
                       .UseStartup<TestStartup>()
                   )
                  .Build();
            _host.Start();

            Services = _host.Services;

            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:5000");
        }

        public HttpClient Client { get; }
        public IServiceProvider Services { get; }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
