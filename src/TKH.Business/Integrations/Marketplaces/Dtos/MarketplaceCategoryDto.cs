namespace TKH.Business.Integrations.Marketplaces.Dtos
{
    public class MarketplaceCategoryDto
    {
        public string ExternalId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? ParentExternalId { get; set; }
        public bool IsLeaf { get; set; }
    }
}
