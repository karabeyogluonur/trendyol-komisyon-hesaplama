using AutoMapper;
using TKH.Core.Common.Settings;
using TKH.Integrations.Trendyol.Settings;
using TKH.Presentation.Features.Settings.Models;

namespace TKH.Presentation.Features.Settings.Mappings
{
    public class SettingsProfile : Profile
    {
        public SettingsProfile()
        {
            CreateMap<TaxSettings, TaxSettingsViewModel>().ReverseMap();
            CreateMap<TrendyolSettings, TrendyolSettingsViewModel>().ReverseMap();
        }
    }
}
