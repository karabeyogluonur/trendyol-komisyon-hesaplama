namespace TKH.Core.Common.Exceptions
{
    public sealed class IntegrationTransientException : IntegrationException
    {
        public IntegrationTransientException(string message, Exception? inner = null)
            : base(message, inner) { }
    }
}
