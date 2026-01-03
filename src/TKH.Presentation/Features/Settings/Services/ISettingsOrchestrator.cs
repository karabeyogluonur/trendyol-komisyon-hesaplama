using TKH.Core.Utilities.Results;
using TKH.Presentation.Features.Settings.Models;
using IResult = TKH.Core.Utilities.Results.IResult;

namespace TKH.Presentation.Features.Settings.Services
{
    public interface ISettingsOrchestrator
    {
        Task<IDataResult<TaxSettingsViewModel>> PrepareTaxSettingsViewModelAsync();
        Task<IResult> UpdateTaxSettingsAsync(TaxSettingsViewModel taxSettingsViewModel);

        Task<IDataResult<TrendyolSettingsViewModel>> PrepareTrendyolSettingsViewModelAsync();
        Task<IResult> UpdateTrendyolSettingsAsync(TrendyolSettingsViewModel trendyolSettingsViewModel);
    }
}
