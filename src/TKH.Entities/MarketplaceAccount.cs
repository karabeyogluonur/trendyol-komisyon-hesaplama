using TKH.Core.Entities.Abstract;
using TKH.Entities.Common;
using TKH.Entities.Enums;

namespace TKH.Entities.Concrete
{
    public class MarketplaceAccount : BaseEntity, IEntity
    {
        public MarketplaceType MarketplaceType { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiSecretKey { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
