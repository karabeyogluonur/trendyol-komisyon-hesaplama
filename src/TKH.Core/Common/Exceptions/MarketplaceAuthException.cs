namespace TKH.Core.Common.Exceptions
{
    public class MarketplaceAuthException : Exception, IMarketplaceApiException
    {
        public MarketplaceAuthException(string message) : base(message) { }
    }
}
