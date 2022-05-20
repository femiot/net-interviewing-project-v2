using Insurance.Api;
using Insurance.Infrastructure.EF;
using Insurance.Shared.AppSettings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;

namespace Insurance.Tests.Helpers
{
    public class TestStartup : Startup
    {
        public TestStartup(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void ConfigureDatabase(IServiceCollection services)
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            services
                   .AddEntityFrameworkSqlite()
                   .AddDbContext<InsuranceContext>(options =>
                   {
                       options.UseSqlite(connection);
                       options.UseInternalServiceProvider(services.BuildServiceProvider());
                   });
            services.AddHealthChecks();
            services.AddTransient<TestDataInitializer>();
        }

        protected override void ConfigureEndpoints(IApplicationBuilder app)
        {
            app.UseEndpoints(
                ep =>
                {
                    ep.MapControllers();
                    ep.MapHealthChecks("/alive");
                    ep.MapGet(
                        "products/{id:int}",
                        context =>
                        {
                            int productId = int.Parse((string)context.Request.RouteValues["id"]);
                            var product = new
                            {
                                id = productId,
                                name = "Test Product",
                                productTypeId = 1,
                                salesPrice = 750
                            };
                            return context.Response.WriteAsync(JsonSerializer.Serialize(product));
                        }
                    );
                    ep.MapGet(
                       "product_types/{id:int}",
                       context =>
                       {
                           int productTypeId = int.Parse((string)context.Request.RouteValues["id"]);
                           var productType = new
                           {
                               id = productTypeId,
                               name = "Test type",
                               canBeInsured = true
                           };
                           return context.Response.WriteAsync(JsonSerializer.Serialize(productType));
                       }
                   );
                    ep.MapGet(
                        "product_types",
                        context =>
                        {
                            var productTypes = new[]
                                               {
                                                   new
                                                   {
                                                       id = 1,
                                                       name = "Test type",
                                                       canBeInsured = true
                                                   },
                                                   new
                                                   {
                                                       id = 2,
                                                       name = "Test type2",
                                                       canBeInsured = true
                                                   },
                                                   new
                                                   {
                                                       id = 3,
                                                       name = "Test type3",
                                                       canBeInsured = false
                                                   }
                                               };
                            return context.Response.WriteAsync(JsonSerializer.Serialize(productTypes));
                        }
                    );
                }
            );
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            base.Configure(app, env, serviceProvider);
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var seeder = serviceScope.ServiceProvider.GetService<TestDataInitializer>();
            seeder.SeedToDoItems();
        }
    }

}
