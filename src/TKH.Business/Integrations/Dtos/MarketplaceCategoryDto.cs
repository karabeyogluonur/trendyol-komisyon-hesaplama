namespace TKH.Business.Integrations.Dtos
{
    public class MarketplaceCategoryDto
    {
        public string MarketplaceCategoryId { get; set; }
        public string Name { get; set; }
        public string? ParentMarketplaceCategoryId { get; set; }
        public bool IsLeaf { get; set; }
    }
}
