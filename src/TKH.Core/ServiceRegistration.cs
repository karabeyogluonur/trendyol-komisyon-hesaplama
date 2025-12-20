using Microsoft.Extensions.DependencyInjection;
using TKH.Core.Utilities.Security.Encryption;

namespace TKH.Core
{
    public static class ServiceRegistration
    {
        public static void AddCoreServices(this IServiceCollection services)
        {
            services.AddDataProtection();
            services.AddScoped<ICipherService, CipherService>();
        }
    }
}
