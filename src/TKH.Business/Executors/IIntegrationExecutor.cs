using Refit;
using TKH.Business.Policies;

namespace TKH.Business.Executors
{
    public interface IIntegrationExecutor
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> action, IIntegrationErrorPolicy errorPolicy);

        Task<T> ExecuteRefitAsync<T>(Func<Task<IApiResponse<T>>> action, IIntegrationErrorPolicy errorPolicy);
    }

}
