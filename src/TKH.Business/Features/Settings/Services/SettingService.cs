using System.Linq.Expressions;
using System.Reflection;
using TKH.Core.Common;
using TKH.Core.DataAccess;
using TKH.Core.Entities.Abstract;
using TKH.Entities;

namespace TKH.Business.Features.Settings.Services
{
    public class SettingService : ISettingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Setting> _settingRepository;

        public SettingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _settingRepository = _unitOfWork.GetRepository<Setting>();
        }

        private string GetSectionName<T>()
        {
            string name = typeof(T).Name;

            if (name.EndsWith("Settings"))
                return name.Substring(0, name.Length - "Settings".Length);

            return name;
        }

        public T LoadSettings<T>() where T : ISettings, new()
        {
            var settings = new T();
            string sectionName = GetSectionName<T>();

            List<Setting> allSettings = _settingRepository.GetAll(predicate: setting => setting.Name.StartsWith(sectionName + "."), disableTracking: true).ToList();

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanWrite || !prop.CanRead) continue;

                string key = $"{sectionName}.{prop.Name}";

                var settingEntity = allSettings.FirstOrDefault(setting => setting.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));

                if (settingEntity is not null)
                {
                    object? convertedValue = CommonHelper.To(settingEntity.Value, prop.PropertyType);
                    prop.SetValue(settings, convertedValue);
                }
            }

            return settings;
        }

        public async Task SaveSettingsAsync<T>(T settings) where T : ISettings
        {
            string sectionName = GetSectionName<T>();

            IList<Setting> existingSettings = await _settingRepository.GetAllAsync(predicate: setting => setting.Name.StartsWith(sectionName + "."), disableTracking: false);

            foreach (var prop in typeof(T).GetProperties())
            {
                if (!prop.CanRead) continue;

                string key = $"{sectionName}.{prop.Name}";
                dynamic value = prop.GetValue(settings)!;
                string valueStr = CommonHelper.To<string>(value);

                var settingEntity = existingSettings.FirstOrDefault(setting => setting.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase));

                if (settingEntity is not null)
                {
                    if (settingEntity.Value != valueStr)
                        settingEntity.Value = valueStr;
                }
                else
                    await _settingRepository.InsertAsync(new Setting { Name = key, Value = valueStr });
            }

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<TProp> GetSettingValueAsync<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector) where TSettings : ISettings, new()
        {
            var propInfo = (PropertyInfo)((MemberExpression)keySelector.Body).Member;
            var sectionName = GetSectionName<TSettings>();

            string key = $"{sectionName}.{propInfo.Name}";

            Setting setting = await _settingRepository.GetFirstOrDefaultAsync(predicate: setting => setting.Name == key);

            if (setting == null)
                return default!;

            return CommonHelper.To<TProp>(setting.Value);
        }

        public async Task SaveSettingAsync<TSettings, TProp>(Expression<Func<TSettings, TProp>> keySelector, TProp value)
            where TSettings : ISettings, new()
        {
            var propInfo = (PropertyInfo)((MemberExpression)keySelector.Body).Member;
            var sectionName = GetSectionName<TSettings>();

            string key = $"{sectionName}.{propInfo.Name}";
            string valueStr = CommonHelper.To<string>(value!);

            Setting setting = await _settingRepository.GetFirstOrDefaultAsync(predicate: setting => setting.Name == key);

            if (setting is not null)
            {
                setting.Value = valueStr;
                _settingRepository.Update(setting);
            }
            else
                await _settingRepository.InsertAsync(new Setting { Name = key, Value = valueStr });

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
