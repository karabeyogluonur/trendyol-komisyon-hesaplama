using System.Reflection;
using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using TKH.Business.Features.MarketplaceAccounts.Services;

namespace TKH.Business.Jobs.Filters
{
    public class MarketplaceJobFailureFilter : JobFilterAttribute, IClientFilter, IServerFilter
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public MarketplaceJobFailureFilter(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void OnCreated(CreatedContext filterContext) { }
        public void OnCreating(CreatingContext filterContext) { }
        public void OnPerforming(PerformingContext filterContext) { }
        public void OnPerformed(PerformedContext filterContext)
        {
            if (filterContext.Job.Args.Count is 0 || filterContext.Job.Args[0] is not int accountId)
                return;

            if (filterContext.Exception is null || filterContext.Canceled)
                return;

            Exception actualException = filterContext.Exception;

            if (actualException is AggregateException aggregateException)
                actualException = aggregateException.Flatten().InnerExceptions.FirstOrDefault() ?? actualException;

            while (actualException.InnerException is not null &&
                  (actualException is TargetInvocationException ||
                   actualException.Message.Contains("An exception occurred during performance")))
            {
                actualException = actualException.InnerException;
            }

            using (IServiceScope scope = _serviceScopeFactory.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<IMarketplaceAccountService>();
                service.MarkMarketplaceAccountSyncFailedAsync(accountId, actualException).GetAwaiter().GetResult();
            }
        }
    }
}
