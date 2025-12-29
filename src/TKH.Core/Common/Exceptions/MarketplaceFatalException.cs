namespace TKH.Core.Common.Exceptions
{
    public class MarketplaceFatalException : Exception, IMarketplaceApiException
    {
        public MarketplaceFatalException(string message) : base(message) { }
    }
}
