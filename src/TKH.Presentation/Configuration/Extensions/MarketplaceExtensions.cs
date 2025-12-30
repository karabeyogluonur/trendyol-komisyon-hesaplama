using TKH.Entities.Enums;

namespace TKH.Presentation.Configuration.Extensions
{
    public static class MarketplaceExtensions
    {
        public static string GetLogoUrl(this MarketplaceType marketplaceType)
        {
            if ((int)marketplaceType == 0) return "/assets/media/marketplace-logos/blank.png";

            return $"/assets/media/marketplace-logos/{marketplaceType.ToString().ToLower()}.png";
        }
    }
}
