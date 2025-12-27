using AutoMapper;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Providers.Trendyol.Profiles
{
    public class TrendyolProductProfile : Profile
    {
        public TrendyolProductProfile()
        {
            CreateMap<TrendyolProductContent, MarketplaceProductDto>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExternalUrl, opt => opt.MapFrom(src => src.ProductUrl ?? string.Empty))
                .ForMember(dest => dest.ExternalProductCode, opt => opt.MapFrom(src => src.ProductCode.ToString()))
                .ForMember(dest => dest.ModelCode, opt => opt.MapFrom(src => src.ProductMainId ?? string.Empty))
                .ForMember(dest => dest.Barcode, opt => opt.MapFrom(src => src.Barcode ?? string.Empty))
                .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.StockCode ?? string.Empty))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Title ?? string.Empty))
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Images != null && src.Images.Count > 0 ? src.Images[0].Url : string.Empty))
                .ForMember(dest => dest.Deci, opt => opt.MapFrom(src => (double)src.DimensionalWeight.GetValueOrDefault(0)))
                .ForMember(dest => dest.VatRate, opt => opt.MapFrom(src => src.VatRate))
                .ForMember(dest => dest.CommissionRate, opt => opt.Ignore())
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.UnitType, opt => opt.MapFrom(src => MapUnitType(src.StockUnitType)))
                .ForMember(dest => dest.IsOnSale, opt => opt.MapFrom(src => src.OnSale))
                .ForMember(dest => dest.IsApproved, opt => opt.MapFrom(src => src.Approved))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.Locked))
                .ForMember(dest => dest.IsArchived, opt => opt.MapFrom(src => src.Archived))
                .ForMember(dest => dest.LastUpdateDateTime, opt => opt.MapFrom(src => src.LastUpdateDate.HasValue ? DateTimeOffset.FromUnixTimeMilliseconds(src.LastUpdateDate.Value).DateTime : DateTime.UtcNow))
                .ForMember(dest => dest.ExternalCategoryId, opt => opt.MapFrom(src => src.PimCategoryId.GetValueOrDefault(0)))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.CategoryName))
                .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes))
                .ForMember(dest => dest.Prices, opt => opt.MapFrom(src => MapPrices(src)))
                .ForMember(dest => dest.Expenses, opt => opt.Ignore())
                .ForMember(dest => dest.MarketplaceAccountId, opt => opt.Ignore());

            CreateMap<TrendyolProductAttribute, MarketplaceProductAttributeDto>()
                .ForMember(dest => dest.ExternalAttributeId, opt => opt.MapFrom(src => src.AttributeId.ToString()))
                .ForMember(dest => dest.AttributeName, opt => opt.MapFrom(src => src.AttributeName))
                .ForMember(dest => dest.ExternalValueId, opt => opt.MapFrom(src => src.AttributeValueId.HasValue ? src.AttributeValueId.Value.ToString() : null))
                .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.AttributeValue));
        }

        private static List<MarketplaceProductPriceDto> MapPrices(TrendyolProductContent src)
        {
            var prices = new List<MarketplaceProductPriceDto>();

            if (src.ListPrice > 0)
                prices.Add(new MarketplaceProductPriceDto { Type = ProductPriceType.ListPrice, Amount = src.ListPrice, IsVatIncluded = true });

            if (src.SalePrice > 0)
                prices.Add(new MarketplaceProductPriceDto { Type = ProductPriceType.SalePrice, Amount = src.SalePrice, IsVatIncluded = true });

            return prices;
        }

        private static ProductUnitType MapUnitType(string unitType)
        {
            if (string.IsNullOrEmpty(unitType)) return ProductUnitType.Piece;

            return unitType.ToLowerInvariant() switch
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
    }
}
