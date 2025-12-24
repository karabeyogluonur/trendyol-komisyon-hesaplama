using System.Text.Json.Serialization;

namespace TKH.Business.Integrations.Providers.Trendyol.Enums
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum TrendyolSettlementTransactionType
    {
        Sale,
        Return,
        Discount,
        DiscountCancel,
        Coupon,
        CouponCancel,
        ProvisionPositive,
        ProvisionNegative,
        ManualRefund,
        ManualRefundCancel,
        TYDiscount,
        TYDiscountCancel,
        TYCoupon,
        TYCouponCancel,
        SellerRevenuePositive,
        SellerRevenueNegative,
        CommissionPositive,
        CommissionNegative,
        SellerRevenuePositiveCancel,
        SellerRevenueNegativeCancel,
        CommissionPositiveCancel,
        CommissionNegativeCancel
    }
}
