using AutoMapper;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Entities;

namespace TKH.Business.Features.FinancialTransactions.Mappings
{
    public class FinancialTransactionProfile : Profile
    {
        public FinancialTransactionProfile()
        {
            CreateMap<MarketplaceFinancialTransactionDto, FinancialTransaction>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src =>
                    src.TransactionDate.Kind == DateTimeKind.Utc
                        ? src.TransactionDate
                        : DateTime.SpecifyKind(src.TransactionDate, DateTimeKind.Utc)));
        }
    }
}
