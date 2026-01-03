using Hangfire;
using Hangfire.PostgreSql;
using TKH.Business.Jobs.Filters;
using TKH.Business.Jobs.Services;
using TKH.Business.Jobs.Workers;
using TKH.Entities.Enums;

namespace TKH.Presentation.Configuration.Extensions
{
    public static class HangfireServiceExtension
    {
        public static void AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire((IServiceProvider provider, IGlobalConfiguration config) =>
            {
                string? connectionString = configuration.GetConnectionString("DefaultConnection");

                config
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UsePostgreSqlStorage(
                        options => options.UseNpgsqlConnection(connectionString),
                        new PostgreSqlStorageOptions
                        {
                            AllowUnsafeValues = true,
                            QueuePollInterval = TimeSpan.FromSeconds(30),
                            PrepareSchemaIfNecessary = true,
                            SchemaName = "hangfire",
                            DistributedLockTimeout = TimeSpan.FromMinutes(1)
                        }
                    )
                    .UseFilter(new MarketplaceJobFailureFilter(
                        provider.GetRequiredService<IServiceScopeFactory>()
                    ));
            });

            services.AddHangfireServer(options =>
            {

                options.Queues = Enum.GetNames(typeof(BackgroundJobQueue))
                    .Select(queueName => queueName.ToLowerInvariant())
                    .ToArray();

                options.WorkerCount = Environment.ProcessorCount;
            });
        }

        public static void RegisterRecurringJobs(this WebApplication app)
        {
            RecurringJob.AddOrUpdate<IMarketplaceJobService>(
                "daily-routine-dispatcher",
                service => service.DispatchScheduledAllAccountsDataSyncAsync(),
                Cron.Daily,
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, MisfireHandling = MisfireHandlingMode.Ignorable }
            );

            RecurringJob.AddOrUpdate<IMarketplaceJobService>(
                "weekly-reference-dispatcher",
                service => service.DispatchMarketplaceCategoryDataSyncAsync(),
                "0 4 * * 0",
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, MisfireHandling = MisfireHandlingMode.Ignorable }
            );

            RecurringJob.AddOrUpdate<InternalCalculationWorkerJob>(
                "product-shipping-cost-analysis",
                service => service.ExecuteProductShippingCostAnalysisAsync(),
                Cron.Daily(4, 0),
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, MisfireHandling = MisfireHandlingMode.Ignorable }
            );

            RecurringJob.AddOrUpdate<InternalCalculationWorkerJob>(
                "product-commission-rate-analysis",
                service => service.ExecuteProductCommissionSyncAsync(),
                Cron.Daily(4, 0),
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, MisfireHandling = MisfireHandlingMode.Ignorable }
            );
        }
    }
}
