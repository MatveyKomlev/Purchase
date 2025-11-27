using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Purchase.Data;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Purchase.Services
{
    public class SimpleAuthService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IDbContextFactory<PurchaseContext> _contextFactory;
        private bool _isJsAvailable = false;

        public SimpleAuthService(IJSRuntime jsRuntime, IDbContextFactory<PurchaseContext> contextFactory)
        {
            _jsRuntime = jsRuntime;
            _contextFactory = contextFactory;
            _isJsAvailable = jsRuntime.GetType().Name != "UnsupportedJavaScriptRuntime";
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            await _jsRuntime.InvokeVoidAsync("console.log", "🔐 SimpleAuthService.LoginAsync started", username);

            if (!_isJsAvailable) return false;

            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();

                var user = await context.Users
                    .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

                if (user == null)
                {
                    await _jsRuntime.InvokeVoidAsync("console.log", "❌ User not found in database");
                    return false;
                }

                await _jsRuntime.InvokeVoidAsync("console.log", "✅ User found:", user.Username);
                await _jsRuntime.InvokeVoidAsync("console.log", "🔑 Stored password:", user.PasswordHash);
                await _jsRuntime.InvokeVoidAsync("console.log", "🔑 Input password:", password);

                // ПРОСТАЯ ПРОВЕРКА - сравниваем как есть
                bool passwordValid = (user.PasswordHash == password);
                await _jsRuntime.InvokeVoidAsync("console.log", "🔍 Simple check result:", passwordValid);

                if (passwordValid)
                {
                    await _jsRuntime.InvokeVoidAsync("console.log", "✅ Password correct! Saving to localStorage...");

                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "isAuthenticated", "true");
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "username", username);
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userRole", user.Role.ToString());
                    await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", user.ID.ToString());

                    // ОБНОВЛЯЕМ ВРЕМЯ ВХОДА (исправленная версия)
                    await UpdateLastLoginAsync(user.ID);

                    await _jsRuntime.InvokeVoidAsync("console.log", "✅ Login successful!");
                    return true;
                }

                await _jsRuntime.InvokeVoidAsync("console.log", "❌ Password check failed");
                return false;
            }
            catch (Exception ex)
            {
                await _jsRuntime.InvokeVoidAsync("console.log", "💥 Exception in LoginAsync:", ex.Message);
                if (ex.InnerException != null)
                {
                    await _jsRuntime.InvokeVoidAsync("console.log", "💥 Inner exception:", ex.InnerException.Message);
                }
                return false;
            }
        }

        private async Task UpdateLastLoginAsync(int userId)
        {
            try
            {
                await using var context = await _contextFactory.CreateDbContextAsync();
                var user = await context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.LastLogin = DateTime.Now;
                    await context.SaveChangesAsync();
                    await _jsRuntime.InvokeVoidAsync("console.log", "✅ Last login updated");
                }
            }
            catch (Exception ex)
            {
                await _jsRuntime.InvokeVoidAsync("console.log", "⚠️ Could not update last login (non-critical):", ex.Message);
                // Это не критическая ошибка, продолжаем
            }
        }

        // Остальные методы без изменений
        public async Task LogoutAsync()
        {
            if (!_isJsAvailable) return;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "isAuthenticated");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "username");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userRole");
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userId");
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            if (!_isJsAvailable) return false;
            try
            {
                var auth = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "isAuthenticated");
                return auth == "true";
            }
            catch { return false; }
        }

        public async Task<string> GetUsernameAsync()
        {
            if (!_isJsAvailable) return "";
            try
            {
                return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "username") ?? "";
            }
            catch { return ""; }
        }

        public async Task<string> GetUserRoleAsync()
        {
            if (!_isJsAvailable) return "";
            try
            {
                return await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userRole") ?? "User";
            }
            catch { return "User"; }
        }

        public async Task<int> GetUserIdAsync()
        {
            if (!_isJsAvailable) return 0;
            try
            {
                var userIdStr = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userId");
                return int.TryParse(userIdStr, out int userId) ? userId : 0;
            }
            catch { return 0; }
        }
    }
}

namespace Purchase.Services
{
    public class SimpleAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly SimpleAuthService _authService;

        public SimpleAuthStateProvider(IJSRuntime jsRuntime, SimpleAuthService authService)
        {
            _jsRuntime = jsRuntime;
            _authService = authService;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                if (!IsJsRuntimeAvailable())
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                var isAuthenticated = await _authService.IsAuthenticatedAsync();
                var username = await _authService.GetUsernameAsync();
                var userRole = await _authService.GetUserRoleAsync();

                if (isAuthenticated && !string.IsNullOrEmpty(username))
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim(ClaimTypes.Role, userRole),
                        new Claim("UserId", (await _authService.GetUserIdAsync()).ToString())
                    };

                    var identity = new ClaimsIdentity(claims, "simple_auth");
                    return new AuthenticationState(new ClaimsPrincipal(identity));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Auth state error: {ex.Message}");
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        private bool IsJsRuntimeAvailable()
        {
            return _jsRuntime != null && _jsRuntime.GetType().Name != "UnsupportedJavaScriptRuntime";
        }

        public void NotifyAuthenticationStateChanged()
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}