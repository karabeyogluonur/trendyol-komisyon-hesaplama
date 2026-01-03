namespace TKH.Core.Common.Exceptions
{
    public sealed class IntegrationAuthException : IntegrationException
    {
        public IntegrationAuthException(string message, Exception? inner = null)
            : base(message, inner) { }
    }
}
