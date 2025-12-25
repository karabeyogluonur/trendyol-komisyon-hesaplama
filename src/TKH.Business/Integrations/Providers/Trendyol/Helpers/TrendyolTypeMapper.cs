using TKH.Business.Integrations.Providers.Trendyol.Enums;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Providers.Trendyol.Helpers
{
    public static class TrendyolTypeMapper
    {
        public static FinancialTransactionType MapSettlement(TrendyolSettlementTransactionType type)
        {
            return type switch
            {
                TrendyolSettlementTransactionType.Sale => FinancialTransactionType.Sale,
                TrendyolSettlementTransactionType.Return => FinancialTransactionType.Return,
                TrendyolSettlementTransactionType.Discount => FinancialTransactionType.Promotion,
                TrendyolSettlementTransactionType.Coupon => FinancialTransactionType.Promotion,
                TrendyolSettlementTransactionType.TYDiscount => FinancialTransactionType.Promotion,
                TrendyolSettlementTransactionType.TYCoupon => FinancialTransactionType.Promotion,
                TrendyolSettlementTransactionType.DiscountCancel => FinancialTransactionType.Promotion,
                TrendyolSettlementTransactionType.CouponCancel => FinancialTransactionType.Promotion,
                TrendyolSettlementTransactionType.CommissionNegative => FinancialTransactionType.Commission,
                TrendyolSettlementTransactionType.CommissionPositive => FinancialTransactionType.Commission,
                TrendyolSettlementTransactionType.CommissionPositiveCancel => FinancialTransactionType.Commission,
                TrendyolSettlementTransactionType.CommissionNegativeCancel => FinancialTransactionType.Commission,
                _ => FinancialTransactionType.Other
            };
        }
        public static FinancialTransactionType MapOther(TrendyolOtherFinancialTransactionType type)
        {
            return type switch
            {
                TrendyolOtherFinancialTransactionType.DeductionInvoices => FinancialTransactionType.ServiceFee,
                TrendyolOtherFinancialTransactionType.Stoppage => FinancialTransactionType.ServiceFee,
                TrendyolOtherFinancialTransactionType.CommissionAgreementInvoice => FinancialTransactionType.ServiceFee,
                TrendyolOtherFinancialTransactionType.ReturnInvoice => FinancialTransactionType.ServiceFee,
                TrendyolOtherFinancialTransactionType.WireTransfer => FinancialTransactionType.Other,
                TrendyolOtherFinancialTransactionType.IncomingTransfer => FinancialTransactionType.Other,
                TrendyolOtherFinancialTransactionType.CashAdvance => FinancialTransactionType.Other,

                _ => FinancialTransactionType.Other
            };
        }
    }
}
