using TKH.DataAccess;
using TKH.Business;

var builder = WebApplication.CreateBuilder(args);
var mvcBuilder = builder.Services.AddControllersWithViews();

if (builder.Environment.IsDevelopment())
    mvcBuilder.AddRazorRuntimeCompilation();

builder.Services.AddDataAccessServices(builder.Configuration);
builder.Services.AddBusinessServices();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
