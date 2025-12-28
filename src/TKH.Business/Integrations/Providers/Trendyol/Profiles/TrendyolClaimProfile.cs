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
                .ForMember(dest => dest.ExternalOrderNumber, opt => opt.MapFrom(src => src.OrderNumber ?? string.Empty))
                .ForMember(dest => dest.ExternalShipmentPackageId, opt => opt.MapFrom(src => src.OrderShipmentPackageId != null ? src.OrderShipmentPackageId.ToString() : string.Empty))
                .ForMember(dest => dest.CargoTrackingNumber, opt => opt.MapFrom(src => src.CargoTrackingNumber != null ? src.CargoTrackingNumber.ToString() : string.Empty))
                .ForMember(dest => dest.ClaimDate, opt => opt.MapFrom(src => src.ClaimDate.ToDateTime()))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToDateTime()))
                .ForMember(dest => dest.LastUpdateDateTime, opt => opt.MapFrom(src => src.LastModifiedDate.ToDateTime()))
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.CustomerFirstName ?? string.Empty))
                .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.CustomerLastName ?? string.Empty))
                .ForMember(dest => dest.CargoProviderName, opt => opt.MapFrom(src => src.CargoProviderName ?? string.Empty))
                .ForMember(dest => dest.CargoSenderNumber, opt => opt.MapFrom(src => src.CargoSenderNumber ?? string.Empty))
                .ForMember(dest => dest.CargoTrackingLink, opt => opt.Ignore())
                .ForMember(dest => dest.RejectedExternalPackageId, opt => opt.MapFrom(src => src.RejectedPackageInfo != null && src.RejectedPackageInfo.PackageId != null ? src.RejectedPackageInfo.PackageId.ToString() : null))
                .ForMember(dest => dest.RejectedCargoTrackingNumber, opt => opt.MapFrom(src => src.RejectedPackageInfo != null && src.RejectedPackageInfo.CargoTrackingNumber != null ? src.RejectedPackageInfo.CargoTrackingNumber.ToString() : null))
                .ForMember(dest => dest.RejectedCargoProviderName, opt => opt.MapFrom(src => src.RejectedPackageInfo != null ? src.RejectedPackageInfo.CargoProviderName : null))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => MapClaimItems(src.Items)));
        }

        private List<MarketplaceClaimItemDto> MapClaimItems(List<TrendyolClaimLineItem>? sourceLines)
        {
            if (sourceLines == null || !sourceLines.Any())
                return new List<MarketplaceClaimItemDto>();

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
