using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace TKH.Integrations.Trendyol.Enums
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
        TyDiscount,
        TyDiscountCancel,
        TyCoupon,
        TyCouponCancel,
        SellerRevenuePositive,
        SellerRevenueNegative,
        CommissionPositive,
        CommissionNegative,
        SellerRevenuePositiveCancel,
        SellerRevenueNegativeCancel,
        CommissionPositiveCancel,
        CommissionNegativeCancel,
        DeliveryFee,
        DeliveryFeeCancel,
        PayByLink
    }
}
