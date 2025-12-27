using AutoMapper;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Providers.Trendyol.Enums;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Providers.Trendyol.Profiles
{
    public class TrendyolOrderProfile : Profile
    {
        public TrendyolOrderProfile()
        {
            CreateMap<TrendyolOrderLine, MarketplaceOrderItemDto>()
                .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.MerchantSku))
                .ForMember(dest => dest.ExternalProductCode, opt => opt.MapFrom(src => src.ProductCode.ToString()))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Barcode))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.VatRate, opt => opt.MapFrom(src => src.VatRate))
                .ForMember(dest => dest.CommissionRate, opt => opt.MapFrom(src => src.Commission))
                .ForMember(dest => dest.PlatformCoveredDiscount, opt => opt.MapFrom(src => src.LineTyDiscount))
                .ForMember(dest => dest.SellerCoveredDiscount, opt => opt.MapFrom(src => src.LineSellerDiscount))
                .ForMember(dest => dest.OrderItemStatus, opt => opt.MapFrom(src => MapOrderItemStatus(src.OrderLineItemStatusName)));

            CreateMap<TrendyolOrderContent, MarketplaceOrderDto>()
                .ForMember(dest => dest.ExternalOrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.ExternalShipmentId, opt => opt.MapFrom(src => src.ShipmentPackageId.ToString()))
                .ForMember(dest => dest.GrossAmount, opt => opt.MapFrom(src => src.GrossAmount))
                .ForMember(dest => dest.TotalDiscount, opt => opt.MapFrom(src => src.TotalDiscount))
                .ForMember(dest => dest.PlatformCoveredDiscount, opt => opt.MapFrom(src => src.PackageTyDiscount.GetValueOrDefault(0)))
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => string.IsNullOrEmpty(src.CurrencyCode) ? "TRY" : src.CurrencyCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => MapOrderStatus(src.Status)))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => DateTimeOffset.FromUnixTimeMilliseconds(src.OrderDate).DateTime))
                .ForMember(dest => dest.CargoTrackingNumber, opt => opt.MapFrom(src => src.CargoTrackingNumber.HasValue ? src.CargoTrackingNumber.ToString() : string.Empty))
                .ForMember(dest => dest.CargoProviderName, opt => opt.MapFrom(src => src.CargoProviderName))
                .ForMember(dest => dest.Deci, opt => opt.MapFrom(src => src.CargoDeci))
                .ForMember(dest => dest.IsShipmentPaidBySeller, opt => opt.MapFrom(src => src.WhoPays.HasValue && (int)src.WhoPays.Value == 1))
                .ForMember(dest => dest.IsMicroExport, opt => opt.MapFrom(src => src.Micro))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Lines))
                .ForMember(dest => dest.MarketplaceAccountId, opt => opt.Ignore());
        }

        private OrderStatus MapOrderStatus(TrendyolOrderStatus status)
        {
            return status switch
            {
                TrendyolOrderStatus.Created => OrderStatus.Created,
                TrendyolOrderStatus.Awaiting => OrderStatus.Awaiting,
                TrendyolOrderStatus.Picking => OrderStatus.Preparing,
                TrendyolOrderStatus.Picked => OrderStatus.Preparing,
                TrendyolOrderStatus.Shipped => OrderStatus.Shipped,
                TrendyolOrderStatus.Delivered => OrderStatus.Delivered,
                TrendyolOrderStatus.Cancelled => OrderStatus.Cancelled,
                TrendyolOrderStatus.Returned => OrderStatus.Returned,
                TrendyolOrderStatus.Returning => OrderStatus.Returned,

                _ => OrderStatus.Unknown
            };
        }

        private OrderItemStatus MapOrderItemStatus(string statusName)
        {
            if (string.IsNullOrEmpty(statusName)) return OrderItemStatus.Created;

            var normalized = statusName.ToLowerInvariant().Trim();

            if (normalized.Contains("unsupplied")) return OrderItemStatus.CancelledBySeller;
            if (normalized.Contains("cancel")) return OrderItemStatus.CancelledByMarketplace;

            if (normalized.Contains("returncreated")) return OrderItemStatus.ReturnRequested;
            if (normalized.Contains("returnshipped")) return OrderItemStatus.ReturnShipped;
            if (normalized.Contains("returndelivered")) return OrderItemStatus.ReturnDeliveredToSeller;
            if (normalized.Contains("returnrejected")) return OrderItemStatus.ReturnRejected;
            if (normalized.Contains("returned")) return OrderItemStatus.Returned;

            if (normalized.Contains("create")) return OrderItemStatus.Created;
            if (normalized.Contains("picking") || normalized.Contains("picked")) return OrderItemStatus.Created;
            if (normalized.Contains("invoice")) return OrderItemStatus.Created;

            if (normalized.Contains("ship")) return OrderItemStatus.Shipped;

            if (normalized.Contains("undeliver")) return OrderItemStatus.Undelivered;
            if (normalized.Contains("deliver")) return OrderItemStatus.Delivered;

            return OrderItemStatus.Other;
        }
    }
}
