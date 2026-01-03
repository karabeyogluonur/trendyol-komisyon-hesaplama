namespace TKH.Core.Common.Exceptions
{
    public abstract class IntegrationException : Exception
    {
        protected IntegrationException(string message, Exception? inner = null)
            : base(message, inner)
        {
        }
    }

}
