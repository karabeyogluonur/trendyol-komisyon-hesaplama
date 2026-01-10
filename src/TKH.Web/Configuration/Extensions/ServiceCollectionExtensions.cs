using FluentValidation.AspNetCore;
using FluentValidation;

using System.Reflection;

using TKH.Business;
using TKH.Core;
using TKH.DataAccess;
using TKH.Core.Contexts;
using TKH.Web.Infrastructure.Services;
using TKH.Integrations.Trendyol.Configuration;
using TKH.Web.Features.MarketplaceAccounts.Services;
using TKH.Web.Features.Products.Services;
using TKH.Web.Infrastructure.Filters;
using TKH.Web.Features.ProductProfits.Services;
using TKH.Web.Features.Settings.Services;
using TKH.Web.Features.Dashboard.Services;

namespace TKH.Web.Configuration.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddCustomMvc(this IServiceCollection services, IWebHostEnvironment environment)
        {
            var mvcBuilder = services.AddControllersWithViews(options =>
            {
                options.Filters.Add<ValidateActiveMarketplaceAccountFilter>();
            });

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
            services.AddScoped<IProductProfitOrchestrator, ProductProfitOrchestrator>();
            services.AddScoped<ISettingsOrchestrator, SettingsOrchestrator>();
            services.AddScoped<IDashboardOrchestrator, DashboardOrchestrator>();
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
