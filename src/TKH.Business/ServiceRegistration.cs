using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TKH.Business.Abstract;
using TKH.Business.Concrete;

namespace TKH.Business
{
    public static class ServiceRegistration
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<IMarketplaceAccountService, MarketplaceAccountService>();
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

        }
    }
}
