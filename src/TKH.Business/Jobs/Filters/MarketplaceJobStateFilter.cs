using System.Reflection;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using TKH.Business.Abstract;

namespace TKH.Business.Jobs.Filters
{
    public class MarketplaceJobStateFilter : JobFilterAttribute, IClientFilter, IServerFilter
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MarketplaceJobStateFilter(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void OnCreated(CreatedContext filterContext) { }

        public void OnCreating(CreatingContext filterContext)
        {
            MarketplaceJobStateAttribute? jobStateAttribute = filterContext.Job.Method.GetCustomAttribute<MarketplaceJobStateAttribute>();

            if (jobStateAttribute == null) return;

            if (filterContext.Job.Args.Count == 0 || filterContext.Job.Args[0] is not int accountId)
                return;

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IMarketplaceAccountService marketplaceAccountService = scope.ServiceProvider.GetRequiredService<IMarketplaceAccountService>();
                marketplaceAccountService.MarkAsSyncing(accountId);
            }
        }

        public void OnPerforming(PerformingContext filterContext) { }

        public void OnPerformed(PerformedContext filterContext)
        {
            MarketplaceJobStateAttribute? jobStateAttribute = filterContext.Job.Method.GetCustomAttribute<MarketplaceJobStateAttribute>();

            if (jobStateAttribute == null) return;

            if (filterContext.Job.Args.Count == 0 || filterContext.Job.Args[0] is not int accountId)
                return;

            Exception? actualException = filterContext.Exception;

            if (actualException != null)
            {
                if (actualException is AggregateException aggregateException)
                {
                    actualException = aggregateException.Flatten().InnerExceptions.FirstOrDefault();
                }

                while (actualException?.InnerException != null &&
                      (actualException is System.Reflection.TargetInvocationException ||
                       actualException.Message.Contains("An exception occurred during performance")))
                {
                    actualException = actualException.InnerException;
                }
            }

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                IMarketplaceAccountService marketplaceAccountService = scope.ServiceProvider.GetRequiredService<IMarketplaceAccountService>();

                marketplaceAccountService.MarkAsIdle(accountId, actualException);
            }
        }
    }
}
