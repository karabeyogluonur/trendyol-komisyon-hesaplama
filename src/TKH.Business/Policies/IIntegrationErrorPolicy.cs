using Refit;
using TKH.Core.Common.Exceptions;

namespace TKH.Business.Policies
{
    public interface IIntegrationErrorPolicy
    {
        IntegrationException Map(Exception exception);
        IntegrationException Map<T>(IApiResponse<T> response);
    }
}
