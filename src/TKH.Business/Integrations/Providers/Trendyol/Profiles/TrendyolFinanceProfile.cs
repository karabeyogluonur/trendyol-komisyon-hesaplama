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
                .ForMember(dest => dest.ExternalTransactionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeMilliseconds(src.TransactionDate).UtcDateTime))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.TransactionType))
                .ForMember(dest => dest.ExternalOrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Credit - src.Debt))
                .ForMember(dest => dest.MarketplaceAccountId, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionType, opt => opt.Ignore());



            CreateMap<TrendyolCargoInvoiceContent, MarketplaceShipmentTransactionDto>()
            .ForMember(dest => dest.ExternalOrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
            .ForMember(dest => dest.ExternalParcelId, opt => opt.MapFrom(src => src.ParcelUniqueId.ToString()))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.Deci, opt => opt.MapFrom(src => src.Desi))
            .ForMember(dest => dest.MarketplaceAccountId, opt => opt.Ignore());
        }
    }
}
