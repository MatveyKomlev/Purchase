using Blazorise;
using Blazorise.Bootstrap5;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Purchase.Data;
using Purchase.Services;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Базовая конфигурация
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// База данных
var connectionString = builder.Configuration.GetConnectionString("PurchaseContext");
builder.Services.AddDbContextFactory<PurchaseContext>(options =>
    options.UseNpgsql(connectionString));

// Сервисы приложения
builder.Services.AddScoped<IProposalService, ProposalService>();
builder.Services.AddScoped<IProposalCatalogService, ProposalCatalogService>();
builder.Services.AddScoped<IProposalMaterialService, ProposalMaterialService>();

// Аутентификация и авторизация
builder.Services.AddAuthenticationCore();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<SimpleAuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, SimpleAuthStateProvider>();

// Blazorise
builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrap5Providers()
    .AddFontAwesomeIcons();

var app = builder.Build();

// Инициализация базы данных
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var contextFactory = services.GetRequiredService<IDbContextFactory<PurchaseContext>>();
        using var context = contextFactory.CreateDbContext();

        // Применяем миграции (если есть)
        await context.Database.MigrateAsync();

        // Инициализируем начальные данные
        SeedData.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при инициализации базы данных");
    }
}

// Базовый pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();