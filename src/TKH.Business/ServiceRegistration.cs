using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TKH.Business.Execution;
using TKH.Business.Executors;
using TKH.Business.Extensions;
using TKH.Business.Features.Categories.Services;
using TKH.Business.Features.Claims.Services;
using TKH.Business.Features.FinancialTransactions.Services;
using TKH.Business.Features.MarketplaceAccounts.Services;
using TKH.Business.Features.Orders.Services;
using TKH.Business.Features.ProductExpenses.Services;
using TKH.Business.Features.Products.Services;
using TKH.Business.Features.Settings.Services;
using TKH.Business.Features.ShipmentTransactions.Services;
using TKH.Business.Infrastructure.Storage.Services;
using TKH.Business.Integrations.Marketplaces.Factories;
using TKH.Business.Integrations.Marketplaces.Handlers;
using TKH.Business.Integrations.Marketplaces.RateLimiting;
using TKH.Business.Jobs.Services;
using TKH.Business.Jobs.Workers;
using TKH.Core.Utilities.Storage;

namespace TKH.Business
{
    public static class ServiceRegistration
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            #region Infrastructure & Core Services

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddSingleton<IMarketplaceRateLimiter, MarketplaceRateLimiter>();
            services.AddScoped<IStorageService, LocalStorageService>();

            #endregion

            #region Settings & Configuration

            services.AddScoped<ISettingService, SettingService>();
            services.AddDatabaseSettings();

            #endregion

            #region Domain Services

            services.AddScoped<IMarketplaceAccountService, MarketplaceAccountService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IFinancialTransactionService, FinancialTransactionService>();

            #endregion

            #region Integration & Synchronization Services

            services.AddScoped<IProductSyncService, ProductSyncService>();
            services.AddScoped<IOrderSyncService, OrderSyncService>();
            services.AddScoped<IClaimSyncService, ClaimSyncService>();
            services.AddScoped<IProductExpenseSyncService, ProductExpenseSyncService>();
            services.AddScoped<IFinancialTransactionSyncService, FinancialTransactionSyncService>();
            services.AddScoped<IShipmentTransactionSyncService, ShipmentTransactionSyncService>();
            services.AddScoped<ICategorySyncService, CategorySyncService>();

            #endregion

            #region Execution & Orchestration

            services.AddScoped<IIntegrationExecutor, IntegrationExecutor>();

            #endregion

            #region Background Jobs & Workers

            services.AddScoped<IMarketplaceJobService, MarketplaceJobService>();
            services.AddScoped<MarketplaceWorkerJob>();

            #endregion

            #region Factories

            services.AddScoped<MarketplaceProviderFactory>();

            #endregion
        }
    }
}
