using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Extensions.Http;
using TKH.Business.Integrations.Marketplaces.Abstract;
using TKH.Business.Integrations.Marketplaces.Handlers;
using TKH.Business.Integrations.Marketplaces.RateLimiting;
using TKH.Business.Integrations.Providers.Trendyol;
using TKH.Entities.Enums;
using TKH.Integrations.Trendyol.Infrastructure;
using TKH.Integrations.Trendyol.Infrastructure.Handlers;
using TKH.Integrations.Trendyol.Policies;
using TKH.Integrations.Trendyol.Providers;

namespace TKH.Integrations.Trendyol.Configuration
{
    public static class ServiceRegistration
    {
        public static void AddTrendyolIntegrationServices(this IServiceCollection services)
        {
            #region Infrastructure

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddTransient<TrendyolHttpLoggingHandler>();
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
            services.AddScoped<IMarketplaceDefaultsProvider, TrendyolDefaultsProvider>();
            services.AddScoped<TrendyolClientFactory>();
            services.AddScoped<TrendyolErrorPolicy>();


            #endregion

            #region HttpClients

            services.AddHttpClient(TrendyolDefaults.HttpClientName)
            .AddHttpMessageHandler(serviceProvider =>
                new MarketplaceRateLimitHandler(
                    serviceProvider.GetRequiredService<IMarketplaceRateLimiter>(),
                    MarketplaceType.Trendyol,
                    serviceProvider.GetRequiredService<ILogger<MarketplaceRateLimitHandler>>()
                ))
            .AddHttpMessageHandler<TrendyolHttpLoggingHandler>()
            .AddPolicyHandler((serviceProvider, request) => GetRetryPolicy(serviceProvider));

            #endregion
        }

        #region Polly Policies

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy(IServiceProvider serviceProvider)
        {
            var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("TrendyolRetryPolicy");

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(
                    retryCount: 3,
                    sleepDurationProvider: (retryAttempt, response, context) =>
                    {
                        if (response?.Result?.Headers?.RetryAfter?.Delta.HasValue == true)
                        {
                            TimeSpan delay = response.Result.Headers.RetryAfter.Delta.Value;
                            return delay.Add(TimeSpan.FromSeconds(1));
                        }
                        double jitter = new Random().NextDouble() * 0.5;
                        return TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromSeconds(jitter);
                    },
                    onRetryAsync: async (outcome, timespan, retryAttempt, context) =>
                    {
                        if (logger != null)
                        {
                            string reason = outcome.Exception != null ? outcome.Exception.Message : outcome.Result.StatusCode.ToString();

                            logger.LogWarning(
                                "Trendyol API request failed. Attempt {RetryAttempt} will be made after {Delay} ms. Reason: {Reason}",
                                retryAttempt,
                                timespan.TotalMilliseconds,
                                reason);
                        }
                        await Task.CompletedTask;
                    }
                );
        }

        #endregion
    }
}
