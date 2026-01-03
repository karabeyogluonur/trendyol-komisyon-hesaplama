namespace TKH.Presentation.Features.Common.Models
{
    public class MarketplaceDefaultsViewModel
    {
        public decimal ServiceFee { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new();
    }
}
