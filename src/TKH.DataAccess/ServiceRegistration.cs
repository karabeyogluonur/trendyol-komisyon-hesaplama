using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TKH.Core.DataAccess;
using TKH.DataAccess.Concrete;
using TKH.DataAccess.Contexts;

namespace TKH.DataAccess
{
    public static class ServiceRegistration
    {
        public static void AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TKHDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));



        }
    }
}
