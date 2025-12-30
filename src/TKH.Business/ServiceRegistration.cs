using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using TKH.Business.Abstract;
using TKH.Business.Concrete;
using TKH.Business.Integrations.Concrete;
using TKH.Business.Integrations.Factories;
using TKH.Business.Integrations.Handlers;
using TKH.Business.Integrations.Providers.Trendyol;
using TKH.Business.Integrations.RateLimiting;
using TKH.Business.Jobs;
using TKH.Business.Utilities.Storage;
using TKH.Core.Utilities.Storage;
using TKH.Entities.Enums;

namespace TKH.Business
{
    public static class ServiceRegistration
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            #region Infrastructure & Core Services

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddTransient<MarketplaceErrorHandler>();

            services.AddSingleton<IMarketplaceRateLimiter>(sp =>
                new MarketplaceRateLimiter(new[]
                {
                    new MarketplaceRateLimitOptions
                    {
                        MarketplaceType = MarketplaceType.Trendyol,
                        PermitLimit = TrendyolDefaults.PermitLimit,
                        QueueLimit = TrendyolDefaults.QueueLimit,
                        ReplenishmentPeriod = TrendyolDefaults.ReplenishmentPeriod,
                        TokensPerPeriod = TrendyolDefaults.TokensPerPeriod
                    }
                })
            );


            #endregion

            #region HttpClients

            services.AddHttpClient(TrendyolDefaults.HttpClientName)
                .AddHttpMessageHandler(sp =>
                    new MarketplaceRateLimitHandler(
                        sp.GetRequiredService<IMarketplaceRateLimiter>(),
                        MarketplaceType.Trendyol))
                .AddHttpMessageHandler<MarketplaceErrorHandler>()
                .AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient();

            #endregion

            #region Business Logic Services

            services.AddScoped<IMarketplaceAccountService, MarketplaceAccountService>();
            services.AddScoped<IProductSyncService, ProductSyncService>();
            services.AddScoped<IOrderSyncService, OrderSyncService>();
            services.AddScoped<IClaimSyncService, ClaimSyncService>();
            services.AddScoped<IFinanceSyncService, FinanceSyncService>();
            services.AddScoped<IMarketplaceReferenceSyncService, MarketplaceReferenceSyncService>();
            services.AddScoped<IFinancialTransactionService, FinancialTransactionService>();
            services.AddScoped<IStorageService, LocalStorageService>();
            services.AddScoped<IMarketplaceJobService, MarketplaceJobService>();
            services.AddScoped<MarketplaceWorkerJob>();
            services.AddScoped<IProductService, ProductService>();

            #endregion

            #region Integration Services & Providers

            services.AddScoped<MarketplaceProviderFactory>();

            services.AddScoped<TrendyolProductProvider>();
            services.AddScoped<TrendyolOrderProvider>();
            services.AddScoped<TrendyolFinanceProvider>();
            services.AddScoped<TrendyolClaimProvider>();
            services.AddScoped<TrendyolReferenceProvider>();

            services.AddSingleton<TrendyolClientFactory>();

            #endregion
        }

        #region Polly Policies

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(response =>
                    response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                );
        }

        #endregion
    }
}
