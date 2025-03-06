using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Purchase.Data;
using Purchase.Services;




var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<PurchaseService>();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var DBContextConnectionString = builder.Configuration.GetConnectionString(nameof(PurchaseContext));
builder.Services.AddDbContext<PurchaseContext>(options => options.UseNpgsql(DBContextConnectionString));
builder.Services.AddScoped<IMyService, PurchaseService>();
var app = builder.Build();

var DBContextConnectionString = builder.Configuration.GetConnectionString(nameof(PurchaseContext));
builder.Services.AddDbContextFactory<PurchaseContext>(options => options.UseNpgsql(DBContextConnectionString));
using var serviceScore = app.Services.CreateScope();
var DBContext = serviceScore.ServiceProvider.GetRequiredService<PurchaseContext>();
DBContext?.Database.Migrate();



builder.Services.AddScoped<IMyService, PurchaseService>();

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
