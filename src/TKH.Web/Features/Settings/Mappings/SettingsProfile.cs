using AutoMapper;
using TKH.Core.Common.Settings;
using TKH.Integrations.Trendyol.Settings;
using TKH.Web.Features.Settings.Models;

namespace TKH.Web.Features.Settings.Mappings
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
