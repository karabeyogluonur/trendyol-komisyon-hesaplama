using TKH.Core.Entities.Abstract;
using TKH.Entities.Enums;

namespace TKH.Business.Features.MarketplaceAccounts.Dtos
{
    public class MarketplaceAccountUpdateDto : IDto
    {
        public int Id { get; set; }
        public MarketplaceType MarketplaceType { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecretKey { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
