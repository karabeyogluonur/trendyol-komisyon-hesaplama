using AutoMapper;
using TKH.Business.Features.Settings.Services;
using TKH.Core.Common.Settings;
using TKH.Core.Utilities.Results;
using TKH.Integrations.Trendyol.Settings;
using TKH.Presentation.Features.Settings.Models;
using IResult = TKH.Core.Utilities.Results.IResult;

namespace TKH.Presentation.Features.Settings.Services
{
    public class SettingsOrchestrator : ISettingsOrchestrator
    {
        private readonly ISettingService _settingService;
        private readonly IMapper _mapper;
        private readonly TaxSettings _taxSettings;
        private readonly TrendyolSettings _trendyolSettings;

        public SettingsOrchestrator(ISettingService settingService, IMapper mapper, TaxSettings taxSettings, TrendyolSettings trendyolSettings)
        {
            _settingService = settingService;
            _mapper = mapper;
            _taxSettings = taxSettings;
            _trendyolSettings = trendyolSettings;
        }

        public Task<IDataResult<TaxSettingsViewModel>> PrepareTaxSettingsViewModelAsync()
        {
            TaxSettingsViewModel taxSettingsViewModel = _mapper.Map<TaxSettingsViewModel>(_taxSettings);
            return Task.FromResult<IDataResult<TaxSettingsViewModel>>(new SuccessDataResult<TaxSettingsViewModel>(taxSettingsViewModel));
        }

        public Task<IDataResult<TrendyolSettingsViewModel>> PrepareTrendyolSettingsViewModelAsync()
        {
            TrendyolSettingsViewModel trendyolSettingsViewModel = _mapper.Map<TrendyolSettingsViewModel>(_trendyolSettings);
            return Task.FromResult<IDataResult<TrendyolSettingsViewModel>>(new SuccessDataResult<TrendyolSettingsViewModel>(trendyolSettingsViewModel));
        }

        public async Task<IResult> UpdateTaxSettingsAsync(TaxSettingsViewModel taxSettingsViewModel)
        {
            TaxSettings taxSettings = _mapper.Map<TaxSettings>(taxSettingsViewModel);

            await _settingService.SaveSettingsAsync(taxSettings);

            return new SuccessResult("Vergi ayarları başarıyla güncellendi.");
        }

        public async Task<IResult> UpdateTrendyolSettingsAsync(TrendyolSettingsViewModel trendyolSettingsViewModel)
        {
            TrendyolSettings trendyolSettings = _mapper.Map<TrendyolSettings>(trendyolSettingsViewModel);

            await _settingService.SaveSettingsAsync(trendyolSettings);

            return new SuccessResult("Trendyol ayarları başarıyla güncellendi.");
        }
    }
}
