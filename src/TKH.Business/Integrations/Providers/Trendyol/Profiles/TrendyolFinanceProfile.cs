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
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src =>
                    DateTimeOffset.FromUnixTimeMilliseconds(src.TransactionDate).UtcDateTime))
                .ForMember(dest => dest.OrderItemBarcode, opt => opt.MapFrom(src => src.Barcode))
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Credit - src.Debt))
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => GetUnifiedTransactionType(src.TransactionType)))
                .ForMember(dest => dest.CommissionAmount, opt => opt.MapFrom(src => CalculateSignedCommission(src.CommissionAmount, src.TransactionType)))
                .ForMember(dest => dest.CommissionRate, opt => opt.MapFrom(src => src.CommissionRate ?? 0))
                .ForMember(dest => dest.MarketplaceAccountId, opt => opt.Ignore());
        }
        private FinancialTransactionType GetUnifiedTransactionType(string trendyolType)
        {
            if (string.IsNullOrEmpty(trendyolType))
                return FinancialTransactionType.Other;

            return trendyolType switch
            {
                "Sale" => FinancialTransactionType.Sale,
                "Return" => FinancialTransactionType.Return,
                "ManualRefund" => FinancialTransactionType.Return,
                "ManualRefundCancel" => FinancialTransactionType.Return,
                "CommissionPositive" => FinancialTransactionType.Commission,
                "CommissionNegative" => FinancialTransactionType.Commission,
                "CommissionAgreementInvoice" => FinancialTransactionType.Commission,
                "Discount" => FinancialTransactionType.Promotion,
                "DiscountCancel" => FinancialTransactionType.Promotion,
                "Coupon" => FinancialTransactionType.Promotion,
                "CouponCancel" => FinancialTransactionType.Promotion,
                "TYDiscount" => FinancialTransactionType.Promotion,
                "TYCoupon" => FinancialTransactionType.Promotion,
                "PaymentOrder" => FinancialTransactionType.Settlement,
                "WireTransfer" => FinancialTransactionType.Settlement,
                "IncomingTransfer" => FinancialTransactionType.Settlement,
                "CashAdvance" => FinancialTransactionType.Settlement,
                "DeductionInvoices" => FinancialTransactionType.ServiceFee,
                "Stoppage" => FinancialTransactionType.ServiceFee,
                "ProvisionPositive" => FinancialTransactionType.ServiceFee,
                "ProvisionNegative" => FinancialTransactionType.ServiceFee,

                _ => FinancialTransactionType.Other
            };
        }

        private decimal CalculateSignedCommission(decimal? commissionAmount, string transactionType)
        {
            decimal amount = commissionAmount ?? 0;
            if (amount == 0) return 0;

            var expenseTypes = new[]
            {
                "Sale",
                "ManualRefund",
                "SellerRevenueNegative",
                "CommissionPositive",
                "SellerRevenuePositiveCancel",
                "CommissionNegativeCancel"
            };

            if (expenseTypes.Contains(transactionType))
                return amount * -1;

            return amount;
        }
    }
}
