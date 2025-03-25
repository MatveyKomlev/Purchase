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
builder.Services.AddBlazorise(options => { options.Immediate = true; }).AddBootstrap5Providers().AddFontAwesomeIcons();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var DBContextConnectionString = builder.Configuration.GetConnectionString(nameof(PurchaseContext));
builder.Services.AddDbContext<PurchaseContext>(options => options.UseNpgsql(DBContextConnectionString));

builder.Services.AddScoped<IMyService, PurchaseService>();

var app = builder.Build();

using var serviceScore = app.Services.CreateScope();
var DBContext = serviceScore.ServiceProvider.GetRequiredService<PurchaseContext>();
DBContext?.Database.Migrate();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();


