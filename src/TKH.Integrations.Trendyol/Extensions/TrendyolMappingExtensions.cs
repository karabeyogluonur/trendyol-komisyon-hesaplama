using TKH.Entities.Enums;
using TKH.Integrations.Trendyol.Enums;

namespace TKH.Integrations.Trendyol.Extensions
{
    public static class TrendyolMappingExtensions
    {
        #region Order & Item Mappings

        public static OrderStatus ToOrderStatus(this TrendyolOrderStatus status) => status switch
        {
            TrendyolOrderStatus.Created => OrderStatus.Created,
            TrendyolOrderStatus.Awaiting => OrderStatus.Awaiting,
            TrendyolOrderStatus.Picking or TrendyolOrderStatus.Picked => OrderStatus.Preparing,
            TrendyolOrderStatus.Shipped => OrderStatus.Shipped,
            TrendyolOrderStatus.Delivered => OrderStatus.Delivered,
            TrendyolOrderStatus.Cancelled => OrderStatus.Cancelled,
            TrendyolOrderStatus.Returned or TrendyolOrderStatus.Returning => OrderStatus.Returned,
            _ => OrderStatus.Unknown
        };

        public static OrderItemStatus ToOrderItemStatus(this string? statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName)) return OrderItemStatus.Created;
            var normalized = statusName.ToLowerInvariant().Trim();

            return normalized switch
            {
                var s when s.Contains("unsupplied") => OrderItemStatus.CancelledBySeller,
                var s when s.Contains("cancel") => OrderItemStatus.CancelledByMarketplace,
                var s when s.Contains("returncreated") => OrderItemStatus.ReturnRequested,
                var s when s.Contains("returnshipped") => OrderItemStatus.ReturnShipped,
                var s when s.Contains("returndelivered") => OrderItemStatus.ReturnDeliveredToSeller,
                var s when s.Contains("returnrejected") => OrderItemStatus.ReturnRejected,
                var s when s.Contains("returned") => OrderItemStatus.Returned,
                var s when s.Contains("ship") => OrderItemStatus.Shipped,
                var s when s.Contains("undeliver") => OrderItemStatus.Undelivered,
                var s when s.Contains("deliver") => OrderItemStatus.Delivered,
                _ => OrderItemStatus.Other
            };
        }

        #endregion

        #region Financial Mappings

        public static FinancialTransactionType ToFinancialTransactionType(this TrendyolSettlementTransactionType type) => type switch
        {
            TrendyolSettlementTransactionType.Sale => FinancialTransactionType.Sale,
            TrendyolSettlementTransactionType.Return => FinancialTransactionType.Return,
            TrendyolSettlementTransactionType.Discount or TrendyolSettlementTransactionType.Coupon or
            TrendyolSettlementTransactionType.TyDiscount or TrendyolSettlementTransactionType.TyCoupon or
            TrendyolSettlementTransactionType.DiscountCancel or TrendyolSettlementTransactionType.CouponCancel => FinancialTransactionType.Promotion,
            TrendyolSettlementTransactionType.CommissionNegative or TrendyolSettlementTransactionType.CommissionPositive or
            TrendyolSettlementTransactionType.CommissionPositiveCancel or TrendyolSettlementTransactionType.CommissionNegativeCancel => FinancialTransactionType.Commission,
            _ => FinancialTransactionType.Other
        };

        public static FinancialTransactionType ToFinancialTransactionType(this TrendyolOtherFinancialTransactionType type) => type switch
        {
            TrendyolOtherFinancialTransactionType.DeductionInvoices or TrendyolOtherFinancialTransactionType.Stoppage or
            TrendyolOtherFinancialTransactionType.CommissionAgreementInvoice or TrendyolOtherFinancialTransactionType.ReturnInvoice => FinancialTransactionType.ServiceFee,
            TrendyolOtherFinancialTransactionType.WireTransfer or TrendyolOtherFinancialTransactionType.IncomingTransfer or
            TrendyolOtherFinancialTransactionType.CashAdvance => FinancialTransactionType.Other,
            _ => FinancialTransactionType.Other
        };

        #endregion

        #region Product & Claim Mappings

        public static ProductUnitType ToProductUnitType(this string? unitType)
        {
            if (string.IsNullOrWhiteSpace(unitType)) return ProductUnitType.Piece;
            return unitType.ToLowerInvariant().Trim() switch
            {
                "adet" => ProductUnitType.Piece,
                "kg" => ProductUnitType.Kilogram,
                "gr" => ProductUnitType.Gram,
                "m" => ProductUnitType.Meter,
                "lt" => ProductUnitType.Liter,
                "paket" => ProductUnitType.Packet,
                "set" => ProductUnitType.Set,
                "çift" => ProductUnitType.Pair,
                _ => ProductUnitType.Piece
            };
        }

        public static ClaimStatus ToClaimStatus(this string? statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName)) return ClaimStatus.Other;

            return statusName.ToLowerInvariant().Trim() switch
            {
                "created" => ClaimStatus.Created,
                "waitinginaction" => ClaimStatus.WaitingInAction,
                "waitingfraudcheck" => ClaimStatus.WaitingFraudCheck,
                "accepted" => ClaimStatus.Accepted,
                "rejected" => ClaimStatus.Rejected,
                "cancelled" => ClaimStatus.Cancelled,
                "inanalysis" => ClaimStatus.InAnalysis,
                "unresolved" => ClaimStatus.Unresolved,
                _ => ClaimStatus.Other
            };
        }

        public static ClaimReasonType ToClaimReasonType(this string? code)
        {
            if (string.IsNullOrWhiteSpace(code)) return ClaimReasonType.Other;
            var normalized = code.ToUpperInvariant().Trim();

            return normalized switch
            {
                "SMALLSIZE" or "BIGSIZE" => ClaimReasonType.Unfit,

                "DAMAGEDITEM" or "WRONGITEM" or "WRONGORDER" or "MISSINGPRODUCT" or "MISSINGPART" or "DIFFERENTITEM" => ClaimReasonType.Defective,

                "DISLIKE" or "BETTERPRICE" or "CHANGEREQUEST" => ClaimReasonType.ChangedMind,

                "UNDELIVERED" or "UNDELIVEREDINT" or "INTLOSTCARGO" or "CANNOTBEDISPATCHED" or "UNSENTREPLACEMENTOUTBOUND" or "COMPENSATION" => ClaimReasonType.DeliveryFailure,

                "ABANDON" => ClaimReasonType.CustomerNotPickedUp,

                "ANALYSISREQUEST" => ClaimReasonType.Analysis,

                _ => ClaimReasonType.Other
            };
        }

        #endregion

        #region Utils

        public static DateTime ToDateTime(this long unixTimeStamp)
            => DateTimeOffset.FromUnixTimeMilliseconds(unixTimeStamp).DateTime;

        #endregion
    }
}
