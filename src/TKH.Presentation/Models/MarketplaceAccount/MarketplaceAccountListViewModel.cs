namespace TKH.Presentation.Models.MarketplaceAccount
{
    public class MarketplaceAccountListViewModel
    {
        public int Id { get; set; }
        public string MarketplaceTypeName { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string MerchantId { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
