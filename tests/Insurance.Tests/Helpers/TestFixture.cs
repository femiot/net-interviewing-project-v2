using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
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
                       b => b.UseUrls("http://localhost:5002")
                             .UseStartup<TestStartup>()
                   )
                  .Build();
            _host.Start();

            Client = new HttpClient();
            Client.BaseAddress = new Uri("http://localhost:5002");
        }

        public HttpClient Client { get; }

        public void Dispose()
        {
            Client.Dispose();
        }
    }
}
