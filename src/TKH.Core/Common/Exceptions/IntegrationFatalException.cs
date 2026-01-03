namespace TKH.Core.Common.Exceptions
{
    public sealed class IntegrationFatalException : IntegrationException
    {
        public IntegrationFatalException(string message, Exception? inner = null)
            : base(message, inner) { }
    }
}
