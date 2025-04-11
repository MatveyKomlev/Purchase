using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Purchase.Data;
using Purchase.Services;
using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services
    .AddBlazorise(options => { options.Immediate = true; })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var DBConnectionString = builder.Configuration.GetConnectionString("PurchaseContext");

builder.Services.AddDbContextFactory<PurchaseContext>(options =>
    options.UseNpgsql(DBConnectionString, option => option.CommandTimeout(60))
           .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTrackingWithIdentityResolution));

builder.Services.AddScoped<IMyService, PurchaseService>();
builder.Services.AddScoped<IProposalCategoryService, ProposalCategoryService>();


var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
var factory = serviceScope.ServiceProvider.GetRequiredService<IDbContextFactory<PurchaseContext>>();

// Создаем контекст через фабрику
await using var DBContext = await factory.CreateDbContextAsync();

await DBContext.Database.MigrateAsync();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();


