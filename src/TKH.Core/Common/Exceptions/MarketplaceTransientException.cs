namespace TKH.Core.Common.Exceptions
{
    public class MarketplaceTransientException : Exception, IMarketplaceApiException
    {
        public MarketplaceTransientException(string message) : base(message) { }
    }
}
