using AutoMapper;
using TKH.Business.Integrations.Dtos;
using TKH.Entities;

namespace TKH.Business.Profiles
{
    public class FinanceProfile : Profile
    {
        public FinanceProfile()
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
