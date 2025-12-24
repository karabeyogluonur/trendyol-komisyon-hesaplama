using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using TKH.Business.Abstract;
using TKH.Business.Concrete;
using TKH.Business.Integrations.Concrete;
using TKH.Business.Integrations.Factories;
using TKH.Business.Integrations.Providers.Trendyol;
using TKH.Business.Utilities.Storage;
using TKH.Core.Utilities.Storage;

namespace TKH.Business
{
    public static class ServiceRegistration
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            #region Infrastructure & Core Services

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddHttpClient(TrendyolDefaults.HttpClientName).AddPolicyHandler(GetRetryPolicy());

            services.AddHttpClient();

            #endregion

            #region Business Logic Services

            services.AddScoped<IMarketplaceAccountService, MarketplaceAccountService>();
            services.AddScoped<IProductSyncService, ProductSyncService>();
            services.AddScoped<IOrderSyncService, OrderSyncService>();
            services.AddScoped<IStorageService, LocalStorageService>();

            #endregion

            #region Integration Services & Providers

            services.AddScoped<MarketplaceProviderFactory>();

            services.AddScoped<TrendyolProductProvider>();
            services.AddScoped<TrendyolOrderProvider>();

            services.AddSingleton<TrendyolClientFactory>();

            #endregion
        }

        #region Private Helpers (Polly Policies)

        private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                .WaitAndRetryAsync(3, retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        #endregion
    }
}
