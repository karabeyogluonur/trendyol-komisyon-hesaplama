using System.Linq.Expressions;
using TKH.Core.Entities.Abstract;

namespace TKH.Business.Features.Settings.Services
{
    public interface ISettingService
    {
        Task<TProp> GetSettingValueAsync<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector)
            where TSettings : ISettings, new();

        T LoadSettings<T>() where T : ISettings, new();

        Task SaveSettingsAsync<T>(T settings) where T : ISettings;

        Task SaveSettingAsync<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector, TProp value)
            where TSettings : ISettings, new();
    }
}
