using AutoMapper;
using TKH.Business.Integrations.Marketplaces.Dtos;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Integrations.Trendyol.Extensions;

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
                .ForMember(dest => dest.OrderItemStatus, opt => opt.MapFrom(src => src.OrderLineItemStatusName.ToOrderItemStatus()));

            CreateMap<TrendyolOrderContent, MarketplaceOrderDto>()
                .ForMember(dest => dest.ExternalOrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.ExternalShipmentId, opt => opt.MapFrom(src => src.ShipmentPackageId.ToString()))
                .ForMember(dest => dest.GrossAmount, opt => opt.MapFrom(src => src.GrossAmount))
                .ForMember(dest => dest.TotalDiscount, opt => opt.MapFrom(src => src.TotalDiscount))
                .ForMember(dest => dest.PlatformCoveredDiscount, opt => opt.MapFrom(src => src.PackageTyDiscount.GetValueOrDefault(0)))
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.CurrencyCode) ? "TRY" : src.CurrencyCode))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToOrderStatus()))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToDateTime()))
                .ForMember(dest => dest.CargoTrackingNumber, opt => opt.MapFrom(src => src.CargoTrackingNumber.HasValue ? src.CargoTrackingNumber.Value.ToString() : string.Empty))
                .ForMember(dest => dest.CargoProviderName, opt => opt.MapFrom(src => src.CargoProviderName))
                .ForMember(dest => dest.Deci, opt => opt.MapFrom(src => src.CargoDeci))
                .ForMember(dest => dest.IsShipmentPaidBySeller, opt => opt.MapFrom(src => src.WhoPays.HasValue && (int)src.WhoPays.Value == 1))
                .ForMember(dest => dest.IsMicroExport, opt => opt.MapFrom(src => src.Micro))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Lines))
                .ForMember(dest => dest.MarketplaceAccountId, opt => opt.Ignore());
        }
    }
}
