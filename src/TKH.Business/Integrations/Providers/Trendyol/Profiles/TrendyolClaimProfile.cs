using AutoMapper;
using TKH.Business.Integrations.Dtos;
using TKH.Business.Integrations.Providers.Trendyol.Extensions;
using TKH.Business.Integrations.Providers.Trendyol.Models;
using TKH.Entities.Enums;

namespace TKH.Business.Integrations.Providers.Trendyol.Profiles
{
    public class TrendyolClaimProfile : Profile
    {
        public TrendyolClaimProfile()
        {
            CreateMap<TrendyolClaimContent, MarketplaceClaimDto>()
                .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ExternalOrderNumber, opt => opt.MapFrom(src => src.OrderNumber))
                .ForMember(dest => dest.ExternalShipmentPackageId, opt => opt.MapFrom(src => src.OrderShipmentPackageId.ToString()))
                .ForMember(dest => dest.ClaimDate, opt => opt.MapFrom(src => src.ClaimDate.ToDateTime()))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToDateTime()))
                .ForMember(dest => dest.LastUpdateDateTime, opt => opt.MapFrom(src => src.LastModifiedDate.ToDateTime()))
                .ForMember(dest => dest.CargoTrackingNumber, opt => opt.MapFrom(src => src.CargoTrackingNumber.ToString()))
                .ForMember(dest => dest.RejectedExternalPackageId, opt => opt.MapFrom(src => src.RejectedPackageInfo != null ? src.RejectedPackageInfo.PackageId.ToString() : null))
                .ForMember(dest => dest.RejectedCargoTrackingNumber, opt => opt.MapFrom(src => src.RejectedPackageInfo != null ? src.RejectedPackageInfo.CargoTrackingNumber.ToString() : null))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => MapClaimItems(src.Items)));
        }

        private List<MarketplaceClaimItemDto> MapClaimItems(List<TrendyolClaimLineItem> sourceLines)
        {
            if (sourceLines == null) return new List<MarketplaceClaimItemDto>();

            return sourceLines
                .Where(line => line.OrderLine != null)
                .SelectMany(line => line.ClaimItems.Select(detail => new MarketplaceClaimItemDto
                {
                    ExternalId = detail.Id,
                    ExternalOrderLineItemId = detail.OrderLineItemId.ToString(),
                    Sku = line.OrderLine.MerchantSku ?? string.Empty,
                    Barcode = line.OrderLine.Barcode ?? string.Empty,
                    ProductName = line.OrderLine.ProductName ?? string.Empty,
                    Price = line.OrderLine.Price,
                    VatRate = line.OrderLine.VatRate,
                    Status = detail.ClaimItemStatus?.Name.ToClaimStatus() ?? ClaimStatus.Other,
                    ReasonCode = detail.CustomerClaimItemReason?.Code ?? string.Empty,
                    ReasonName = detail.CustomerClaimItemReason?.Name ?? string.Empty,
                    ReasonType = (detail.CustomerClaimItemReason?.Code ?? detail.TrendyolClaimItemReason?.Code).ToClaimReasonType(),
                    CustomerNote = detail.CustomerNote ?? string.Empty,
                    IsResolved = detail.Resolved.GetValueOrDefault(),
                    IsAutoAccepted = detail.AutoAccepted.GetValueOrDefault(),
                    IsAcceptedBySeller = detail.AcceptedBySeller.GetValueOrDefault()
                }))
                .ToList();
        }
    }
}
