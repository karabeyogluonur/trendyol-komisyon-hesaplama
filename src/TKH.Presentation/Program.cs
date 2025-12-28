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

var builder = WebApplication.CreateBuilder(args);
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

app.UseStatusCodePagesWithReExecute("/Error/404");
app.UseMiddleware<ExceptionMiddleware>();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseHangfireDashboard();

app.RegisterRecurringJobs();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
