using AutoMapper;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Providers.Trendyol.Profiles
{
    public class TrendyolFinancialProfile : Profile
    {
        public TrendyolFinancialProfile()
        {
            CreateMap<TrendyolFinancialContent, MarketplaceFinancialTransactionDto>()
                .ForMember(dest => dest.MarketplaceTransactionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeMilliseconds(src.TransactionDate).UtcDateTime))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.TransactionType))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Credit - src.Debt))
                .ForMember(dest => dest.MarketplaceAccountId, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionType, opt => opt.Ignore());
        }
    }
}
