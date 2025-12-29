using TKH.DataAccess;
using TKH.Business;
using TKH.Core;
using FluentValidation.AspNetCore;
using FluentValidation;
using System.Reflection;
using TKH.Presentation.Services;
using TKH.Business.Abstract;
using TKH.Presentation.Extensions;
using Hangfire;
using TKH.Core.Extensions;
using TKH.Presentation.Middlewares;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Uygulama başlatılıyor...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    var mvcBuilder = builder.Services.AddControllersWithViews();

    if (builder.Environment.IsDevelopment())
        mvcBuilder.AddRazorRuntimeCompilation();

    #region Layer Service Registration
    builder.Services.AddCoreServices();
    builder.Services.AddDataAccessServices(builder.Configuration);
    builder.Services.AddBusinessServices();
    #endregion

    #region Presentation Service Registration
    builder.Services.AddScoped<INotificationService, NotificationService>();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<IWorkContext, WebWorkContext>();
    #endregion

    #region Hangfire Services
    builder.Services.AddHangfireServices(builder.Configuration);
    #endregion

    #region Fluent Validation
    builder.Services.AddFluentValidationAutoValidation(config =>
    {
        config.DisableDataAnnotationsValidation = true;
    });
    builder.Services.AddFluentValidationClientsideAdapters();
    builder.Services.AddValidatorsFromAssemblyContaining<Program>();
    #endregion

    #region Automapper
    builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
    #endregion

    var app = builder.Build();

    app.UseSerilogRequestLogging(opts =>
 {
     opts.GetLevel = (httpContext, elapsed, ex) =>
     {
         if (httpContext.Request.Path.StartsWithSegments("/hangfire"))
             return LogEventLevel.Verbose;

         return LogEventLevel.Information;
     };
 });

    app.UseStatusCodePagesWithReExecute("/Error/404");

    if (!app.Environment.IsDevelopment())
        app.UseHsts();

    app.UseHttpsRedirection();
    app.UseRouting();

    app.UseAuthorization();

    app.UseMiddleware<LogContextMiddleware>();
    app.UseMiddleware<ExceptionMiddleware>();

    app.UseHangfireDashboard();
    app.RegisterRecurringJobs();
    app.MapStaticAssets();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
        .WithStaticAssets();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama beklenmedik bir şekilde sonlandı!");
}
finally
{
    Log.CloseAndFlush();
}
