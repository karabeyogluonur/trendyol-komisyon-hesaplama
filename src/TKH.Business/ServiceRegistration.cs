using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace TKH.Business
{
    public static class ServiceRegistration
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());

        }
    }
}
