using Hangfire;
using Hangfire.PostgreSql;
using TKH.Business.Abstract;
using TKH.Entities.Enums;

namespace TKH.Presentation.Extensions
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
                    );
            });

            services.AddHangfireServer(options =>
            {

                options.Queues = Enum.GetNames(typeof(BackgroundJobQueue))
                    .Select(queueName => queueName.ToLowerInvariant())
                    .ToArray();

                options.WorkerCount = System.Environment.ProcessorCount * 2;
            });
        }

        public static void RegisterRecurringJobs(this WebApplication app)
        {
            RecurringJob.AddOrUpdate<IMarketplaceJobService>(
                "hourly-routine-dispatcher",
                service => service.DispatchScheduledAllAccountsDataSyncAsync(),
                Cron.Hourly,
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, MisfireHandling = MisfireHandlingMode.Ignorable }
            );

            RecurringJob.AddOrUpdate<IMarketplaceJobService>(
                "weekly-reference-dispatcher",
                service => service.DispatchMarketplaceReferenceDataSyncAsync(),
                "0 4 * * 0",
                new RecurringJobOptions { TimeZone = TimeZoneInfo.Local, MisfireHandling = MisfireHandlingMode.Ignorable }
            );
        }
    }
}
