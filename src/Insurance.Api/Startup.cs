using FluentValidation.AspNetCore;
using Insurance.Api.Filters;
using Insurance.Api.Middlewares;
using Insurance.Core.Interfaces;
using Insurance.Core.Services;
using Insurance.Infrastructure.EF;
using Insurance.Infrastructure.Repositories;
using Insurance.Integration.Product.Concrete;
using Insurance.Shared.AppSettings;
using Insurance.Shared.Constants;
using Insurance.Shared.Validators;
using MicroFinanceSystem.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;

namespace Insurance.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => 
            {
                options.Filters.Add<RequestFilter>();
                options.Filters.Add<ExceptionFilter>();
            })
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<SurchargeUploadRequestValidator>());

            services.AddHttpContextAccessor();

            services.AddScoped<ICalculatorService, CalculatorService>();
            services.AddScoped<ISurchargeService, SurchargeService>();

            services.AddScoped<IProductIntegration, ProductIntegration>();
            services.AddScoped<IProductTypeIntegration, ProductTypeIntegration>();

            services.AddScoped<IInsuranceUnitOfWork, InsuranceUnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.Configure<AppConfiguration>(options => Configuration.GetSection(nameof(AppConfiguration)).Bind(options));

            var databaseConnectionString = Configuration.GetConnectionString("database");

            services.AddDbContext<InsuranceContext>(options => options.UseSqlite(databaseConnectionString));

            services.AddHealthChecks().AddSqlite(databaseConnectionString);

            services.AddHttpClient(IntegrationConstants.ProductDataApi, httpClient =>
            {
                httpClient.BaseAddress = new Uri(Configuration[$"{nameof(AppConfiguration)}:{nameof(AppConfiguration.ProductIntegrationServiceUrl)}"]);
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Insurance API",
                    Description = "A .Net 6 Web API for calculating insurance costs on orders",
                    TermsOfService = new Uri("https://www.coolblue.nl/en/c/terms-and-conditions.html"),
                    Contact = new OpenApiContact
                    {
                        Name = "Speak to us",
                        Url = new Uri("https://aboutcoolblue.com/news/contact/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "License",
                        Url = new Uri("https://www.coolblue.nl/en/c/terms-and-conditions.html")
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<CorrelationIdMiddleware>();

            app.UseMiddleware<LoggerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
            });

            try
            {
                UpdateDatabases(app);
            }
            catch (Exception ex)
            {
                var logger = serviceProvider.GetRequiredService<ILogger<Startup>>();
                logger.LogError(ex, "An error occurred with the DB migration.");
            }
        }

        private static void UpdateDatabases(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<InsuranceContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
