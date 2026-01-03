using Refit;
using TKH.Business.Executors;
using TKH.Business.Policies;

namespace TKH.Business.Execution
{
    public sealed class IntegrationExecutor : IIntegrationExecutor
    {
        public async Task<T> ExecuteAsync<T>(Func<Task<T>> action, IIntegrationErrorPolicy errorPolicy)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            if (errorPolicy is null)
                throw new ArgumentNullException(nameof(errorPolicy));

            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                throw errorPolicy.Map(ex);
            }
        }

        public async Task<T> ExecuteRefitAsync<T>(Func<Task<IApiResponse<T>>> action, IIntegrationErrorPolicy errorPolicy)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            if (errorPolicy is null)
                throw new ArgumentNullException(nameof(errorPolicy));

            try
            {
                var response = await action();

                if (!response.IsSuccessful)
                    throw errorPolicy.Map(response);

                return response.Content!;
            }
            catch (Exception ex)
            {
                throw errorPolicy.Map(ex);
            }
        }
    }
}
