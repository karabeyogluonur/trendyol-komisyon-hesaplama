using Hangfire;
using Serilog;
using Serilog.Events;
using TKH.Core.Extensions;
using TKH.Presentation.Middlewares;

namespace TKH.Presentation.Extensions
{
    public static class MiddlewareRegistrationExtensions
    {
        public static void UseCustomSerilogRequestLogging(this WebApplication app)
        {
            app.UseSerilogRequestLogging(opts =>
            {
                opts.GetLevel = (httpContext, elapsed, ex) =>
                {
                    if (httpContext.Request.Path.StartsWithSegments("/hangfire"))
                        return LogEventLevel.Verbose;

                    if (ex != null || httpContext.Response.StatusCode >= 500)
                        return LogEventLevel.Error;

                    return LogEventLevel.Information;
                };
            });
        }

        public static void UseCustomErrorHandling(this WebApplication app)
        {
            app.UseStatusCodePagesWithReExecute("/Error/404");
            app.UseMiddleware<ExceptionMiddleware>();
        }

        public static void UseCustomSecurityAndRouting(this WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
                app.UseHsts();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
        }

        public static void UseCustomMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<LogContextMiddleware>();
        }

        public static void ConfigureHangfire(this WebApplication app)
        {
            app.UseHangfireDashboard();
            app.RegisterRecurringJobs();
        }

        public static void MapCustomEndpoints(this WebApplication app)
        {
            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
        }
    }
}
