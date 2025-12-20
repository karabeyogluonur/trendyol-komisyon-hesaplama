using TKH.Core.Entities.Abstract;

namespace TKH.Business.Dtos.Marketplace
{
    public class MarketplaceAccountListDto : IDto
    {
        public int Id { get; set; }
        public string MarketplaceTypeName { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
