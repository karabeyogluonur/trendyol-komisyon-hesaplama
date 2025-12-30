using TKH.Entities.Enums;

namespace TKH.Business.Features.MarketplaceAccounts.Dtos
{
    public record MarketplaceAccountConnectionDetailsDto
    {
        public int Id { get; init; }
        public string MerchantId { get; init; } = string.Empty;
        public string ApiKey { get; init; } = string.Empty;
        public string ApiSecretKey { get; init; } = string.Empty;
        public MarketplaceType MarketplaceType { get; set; }
    }
}


