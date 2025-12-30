using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Handlers;
using TKH.Business.Integrations.Marketplaces.RateLimiting;
using TKH.Business.Integrations.Providers.Trendyol;
using TKH.Entities.Enums;
using TKH.Integrations.Trendyol.Infrastructure;
using TKH.Integrations.Trendyol.Infrastructure.Handlers;
using TKH.Integrations.Trendyol.Providers;

namespace TKH.Integrations.Trendyol.Configuration
{
    public static class ServiceRegistration
    {
        public static void AddTrendyolIntegrationServices(this IServiceCollection services)
        {
            #region Infrastructure

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddTransient<TrendyolErrorHandler>();
            services.AddSingleton(new MarketplaceRateLimitOptions
            {
                MarketplaceType = MarketplaceType.Trendyol,
                PermitLimit = TrendyolDefaults.PermitLimit,
                QueueLimit = TrendyolDefaults.QueueLimit,
                ReplenishmentPeriod = TrendyolDefaults.ReplenishmentPeriod,
                TokensPerPeriod = TrendyolDefaults.TokensPerPeriod
            });

            #endregion

            #region Providers (Integration Implementations)

            services.AddScoped<IMarketplaceProductProvider, TrendyolProductProvider>();
            services.AddScoped<IMarketplaceOrderProvider, TrendyolOrderProvider>();
            services.AddScoped<IMarketplaceFinanceProvider, TrendyolFinanceProvider>();
            services.AddScoped<IMarketplaceClaimProvider, TrendyolClaimProvider>();
            services.AddScoped<IMarketplaceCategoryProvider, TrendyolCategoryProvider>();

            services.AddSingleton<TrendyolClientFactory>();

            #endregion

            #region HttpClients

            services.AddHttpClient(TrendyolDefaults.HttpClientName)
                    .AddHttpMessageHandler(sp =>
                        new MarketplaceRateLimitHandler(
                            sp.GetRequiredService<IMarketplaceRateLimiter>(),
                            MarketplaceType.Trendyol
                        ))
                    .AddHttpMessageHandler<TrendyolErrorHandler>()
                    .AddPolicyHandler(GetRetryPolicy());

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
