using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;
using TKH.Business;
using TKH.Core;
using TKH.DataAccess;
using TKH.Core.Contexts;
using TKH.Presentation.Infrastructure.Services;
using TKH.Integrations.Trendyol.Configuration;
using TKH.Presentation.Features.MarketplaceAccounts.Services;
using TKH.Presentation.Features.Products.Services;

namespace TKH.Presentation.Configuration.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomMvc(this IServiceCollection services, IWebHostEnvironment environment)
        {
            var mvcBuilder = services.AddControllersWithViews();

            if (environment.IsDevelopment())
                mvcBuilder.AddRazorRuntimeCompilation();
        }

        public static void AddArchitectureLayers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCoreServices();
            services.AddDataAccessServices(configuration);
            services.AddBusinessServices();
            services.AddTrendyolIntegrationServices();
        }

        public static void AddPresentationInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<INotificationService, NotificationService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IWorkContext, WebWorkContext>();
        }

        public static void AddOrchestratorInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IMarketplaceAccountOrchestrator, MarketplaceAccountOrchestrator>();
            services.AddScoped<IProductOrchestrator, ProductOrchestrator>();
        }

        public static void AddCustomValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation(config =>
            {
                config.DisableDataAnnotationsValidation = true;
            });
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public static void AddCustomMapping(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }
    }
}
