using Serilog;
using TKH.Presentation.Configuration.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Application is starting...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddCustomMvc(builder.Environment);
    builder.Services.AddArchitectureLayers(builder.Configuration);
    builder.Services.AddPresentationInfrastructure();
    builder.Services.AddHangfireServices(builder.Configuration);
    builder.Services.AddCustomValidation();
    builder.Services.AddCustomMapping();

    var app = builder.Build();

    await app.InitialiseDatabaseAsync();

    app.UseCustomSerilogRequestLogging();
    app.UseCustomErrorHandling();
    app.UseCustomSecurityAndRouting();
    app.UseCustomMiddlewares();
    app.ConfigureHangfire();
    app.MapCustomEndpoints();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}
