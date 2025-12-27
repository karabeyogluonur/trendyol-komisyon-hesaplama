using AutoMapper;
using TKH.Business.Integrations.Dtos;
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
                .ForMember(dest => dest.ClaimDate, opt => opt.MapFrom(src => UnixTimeStampToDateTime(src.ClaimDate)))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => UnixTimeStampToDateTime(src.OrderDate)))
                .ForMember(dest => dest.LastUpdateDateTime, opt => opt.MapFrom(src => UnixTimeStampToDateTime(src.LastModifiedDate)))
                .ForMember(dest => dest.CustomerFirstName, opt => opt.MapFrom(src => src.CustomerFirstName ?? string.Empty))
                .ForMember(dest => dest.CustomerLastName, opt => opt.MapFrom(src => src.CustomerLastName ?? string.Empty))
                .ForMember(dest => dest.CargoTrackingNumber, opt => opt.MapFrom(src => src.CargoTrackingNumber.HasValue ? src.CargoTrackingNumber.Value.ToString() : string.Empty))
                .ForMember(dest => dest.CargoProviderName, opt => opt.MapFrom(src => src.CargoProviderName ?? string.Empty))
                .ForMember(dest => dest.CargoSenderNumber, opt => opt.MapFrom(src => src.CargoSenderNumber ?? string.Empty))
                .ForMember(dest => dest.CargoTrackingLink, opt => opt.MapFrom(src => src.CargoTrackingLink ?? string.Empty))
                .ForMember(dest => dest.RejectedExternalPackageId, opt => opt.MapFrom(src => src.RejectedPackageInfo != null ? src.RejectedPackageInfo.PackageId.ToString() : null))
                .ForMember(dest => dest.RejectedCargoTrackingNumber, opt => opt.MapFrom(src => src.RejectedPackageInfo != null && src.RejectedPackageInfo.CargoTrackingNumber.HasValue ? src.RejectedPackageInfo.CargoTrackingNumber.Value.ToString() : null))
                .ForMember(dest => dest.RejectedCargoProviderName, opt => opt.MapFrom(src => src.RejectedPackageInfo != null ? src.RejectedPackageInfo.CargoProviderName : null))
                .ForMember(dest => dest.RejectedCargoTrackingLink, opt => opt.MapFrom(src => src.RejectedPackageInfo != null ? src.RejectedPackageInfo.CargoTrackingLink : null))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => MapClaimItems(src.Items)));
        }

        private List<MarketplaceClaimItemDto> MapClaimItems(List<TrendyolClaimLineItem> sourceLines)
        {
            var result = new List<MarketplaceClaimItemDto>();

            if (sourceLines == null || !sourceLines.Any())
                return result;

            foreach (var line in sourceLines)
            {
                if (line.OrderLine == null) continue;

                foreach (var detail in line.ClaimItems)
                {
                    string? reasonCodeForEnum = detail.CustomerClaimItemReason?.Code ?? detail.TrendyolClaimItemReason?.Code;

                    var itemDto = new MarketplaceClaimItemDto
                    {
                        ExternalId = detail.Id,
                        ExternalOrderLineItemId = detail.OrderLineItemId.ToString(),
                        Sku = line.OrderLine.MerchantSku ?? string.Empty,
                        Barcode = line.OrderLine.Barcode ?? string.Empty,
                        ProductName = line.OrderLine.ProductName ?? string.Empty,
                        Price = line.OrderLine.Price,
                        VatRate = line.OrderLine.VatRate,
                        Status = ParseClaimStatus(detail.ClaimItemStatus?.Name),
                        ReasonType = ParseReasonType(reasonCodeForEnum),
                        ReasonCode = detail.CustomerClaimItemReason?.Code ?? string.Empty,
                        ReasonName = detail.CustomerClaimItemReason?.Name ?? string.Empty,
                        CustomerNote = detail.CustomerNote ?? string.Empty,
                        IsResolved = detail.Resolved.GetValueOrDefault(),
                        IsAutoAccepted = detail.AutoAccepted.GetValueOrDefault(),
                        IsAcceptedBySeller = detail.AcceptedBySeller.GetValueOrDefault()
                    };

                    result.Add(itemDto);
                }
            }
            return result;
        }

        private DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTimeStamp).DateTime;
        }

        private ClaimStatus ParseClaimStatus(string? statusName)
        {
            if (string.IsNullOrEmpty(statusName)) return ClaimStatus.Other;

            return statusName.ToLowerInvariant() switch
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

        private ClaimReasonType ParseReasonType(string? code)
        {
            if (string.IsNullOrEmpty(code))
                return ClaimReasonType.Other;

            return code.ToUpperInvariant().Trim() switch
            {
                "SMALLSIZE" => ClaimReasonType.Unfit,
                "BIGSIZE" => ClaimReasonType.Unfit,

                "DAMAGEDITEM" => ClaimReasonType.Defective,
                "WRONGITEM" => ClaimReasonType.Defective,
                "WRONGORDER" => ClaimReasonType.Defective,
                "MISSINGPRODUCT" => ClaimReasonType.Defective,
                "MISSINGPART" => ClaimReasonType.Defective,
                "DIFFERENTITEM" => ClaimReasonType.Defective,

                "DISLIKE" => ClaimReasonType.ChangedMind,
                "BETTERPRICE" => ClaimReasonType.ChangedMind,
                "CHANGEREQUEST" => ClaimReasonType.ChangedMind,

                "UNDELIVERED" => ClaimReasonType.DeliveryFailure,
                "UNDELIVEREDINT" => ClaimReasonType.DeliveryFailure,
                "INTLOSTCARGO" => ClaimReasonType.DeliveryFailure,
                "CANNOTBEDISPATCHED" => ClaimReasonType.DeliveryFailure,
                "UNSENTREPLACEMENTOUTBOUND" => ClaimReasonType.DeliveryFailure,
                "COMPENSATION" => ClaimReasonType.DeliveryFailure,

                "ABANDON" => ClaimReasonType.CustomerNotPickedUp,

                "ANALYSISREQUEST" => ClaimReasonType.Analysis,

                "WRONGCHANGEREQUEST" => ClaimReasonType.Other,
                "OTHER" => ClaimReasonType.Other,

                _ => ClaimReasonType.Other
            };
        }
    }
}
