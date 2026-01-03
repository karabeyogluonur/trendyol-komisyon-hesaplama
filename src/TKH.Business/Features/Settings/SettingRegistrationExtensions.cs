using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TKH.Business.Features.Settings.Services;
using TKH.Core.Entities.Abstract;

namespace TKH.Business.Extensions
{
    public static class SettingRegistrationExtensions
    {
        public static IServiceCollection AddDatabaseSettings(this IServiceCollection services)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(assemblies => assemblies.FullName != null && assemblies.FullName.StartsWith("TKH")).ToList();

            var settingTypes = assemblies.SelectMany(a => a.GetTypes()).Where(type => typeof(ISettings).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract).ToList();

            if (!settingTypes.Any()) return services;

            foreach (var type in settingTypes)
            {
                services.AddScoped(type, serviceProvider =>
                {
                    ISettingService settingService = serviceProvider.GetRequiredService<ISettingService>();

                    MethodInfo? method = settingService.GetType().GetMethod(nameof(ISettingService.LoadSettings));
                    MethodInfo? genericMethod = method?.MakeGenericMethod(type);

                    return genericMethod!.Invoke(settingService, null)!;
                });
            }

            return services;
        }
    }
}
